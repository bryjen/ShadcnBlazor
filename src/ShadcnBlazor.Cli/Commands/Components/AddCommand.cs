using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ShadcnBlazor.Cli.Commands.Components;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class AddCommand(
    FileSystemService fileSystemService,
    ProjectValidator projectValidator,
    ProjectNamespaceService projectNamespaceService,
    ComponentService componentService,
    SharedInfrastructureService sharedInfrastructureService,
    NamespaceService namespaceService,
    UsingService usingService,
    IAnsiConsole console)
    : Command<AddCommand.AddSettings>
{
    private const string ComponentsOutputDir = "./Components/Core";

    public class AddSettings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("The name of the component to add. (input, textarea, checkbox, etc.)")]
        public required string Name { get; init; }

        [CommandOption("-a|--all")]
        [Description("Adds all available components.")]
        [DefaultValue("false")]
        public bool AddAllComponents { get; init; } = false;

        [CommandOption("--silent")]
        [Description("Mutes output.")]
        [DefaultValue("false")]
        public bool Silent { get; init; } = false;

        [CommandOption("--overwrite")]
        [Description("Overwrite if component already exists (no prompt).")]
        [DefaultValue("false")]
        public bool Overwrite { get; init; } = false;
    }

    public override int Execute(CommandContext context, AddSettings settings, CancellationToken cancellation)
    {
        try
        {
            var cwd = Directory.GetCurrentDirectory();
            var cwdInfo = new DirectoryInfo(cwd);

            var csprojFile = projectValidator.ValidateAndGetCsproj(cwdInfo);
            var blazorProjectType = projectValidator.ValidateBlazorProject(csprojFile);
            var rootNamespace = projectNamespaceService.GetRootNamespace(csprojFile);

            var outputProjectConfig = new OutputProjectConfig
            {
                ComponentsOutputDir = ComponentsOutputDir,
                RootNamespace = rootNamespace
            };

            var components = componentService.LoadComponents();
            var componentToAdd = componentService.FindComponent(components, settings.Name);

            var dependencyTree = ComponentDependencyTree.BuildComponentDependencyTree(
                outputProjectConfig, components, componentToAdd.Name.Trim().ToLower());

            var srcDirInfo = componentService.GetComponentsSourceDirectory();

            AddComponentWithDependencies(outputProjectConfig, cwdInfo, srcDirInfo, dependencyTree, blazorProjectType, csprojFile, settings.Silent, settings.Overwrite);

            return 0;
        }
        catch (CliException ex)
        {
            console.MarkupLine($"[red]{ex.Message}[/]");
            console.MarkupLine("Component addition cancelled.");
            return 1;
        }
    }

    private void AddComponentWithDependencies(
        OutputProjectConfig outputProjectConfig,
        DirectoryInfo cwdInfo,
        DirectoryInfo srcDirInfo,
        ComponentDependencyTree componentDependencyTree,
        BlazorProjectType blazorProjectType,
        FileInfo csprojFile,
        bool silent,
        bool overwrite = false)
    {
        void AddComponentWithDependenciesCore(ComponentDependencyNode componentDependencyNode)
        {
            foreach (var dependency in componentDependencyNode.ResolvedDependencies)
            {
                AddComponentWithDependenciesCore(dependency);
            }

            AddComponent(outputProjectConfig, cwdInfo, srcDirInfo, componentDependencyNode.Component, blazorProjectType, csprojFile, silent, overwrite);
        }

        AddComponentWithDependenciesCore(componentDependencyTree.RootNode);
    }

    private void AddComponent(
        OutputProjectConfig outputProjectConfig,
        DirectoryInfo cwdInfo,
        DirectoryInfo srcDirInfo,
        ComponentDefinition definition,
        BlazorProjectType blazorProjectType,
        FileInfo csprojFile,
        bool silent,
        bool overwrite = false)
    {
        var destinationDir = new DirectoryInfo(Path.Join(cwdInfo.FullName, outputProjectConfig.ComponentsOutputDir, definition.Name));
        if (destinationDir.Exists)
        {
            if (silent && !overwrite)
            {
                console.MarkupLine($"[yellow]Skipping {definition.Name} (already exists).[/]");
                return;
            }

            if (!overwrite && PromptUtils.PromptOverwriteComponent(definition.Name) == ConfirmationResponse.No)
            {
                console.MarkupLine($"[yellow]Skipped {definition.Name}.[/]");
                return;
            }

            destinationDir.Delete(recursive: true);
        }

        var sourceDir = new DirectoryInfo(Path.Join(srcDirInfo.FullName, definition.Name));
        if (!sourceDir.Exists)
        {
            throw new ComponentSourceNotFoundException(definition.Name);
        }

        console.MarkupLine($"Adding [green]{definition.Name}[/]...");
        fileSystemService.CopyDirectory(sourceDir.FullName, destinationDir.FullName);

        var targetNamespace = BuildTargetNamespace(outputProjectConfig, definition.Name);
        UpdateNamespacesInComponent(destinationDir, targetNamespace, outputProjectConfig);

        if (definition.Name == "Shared")
        {
            sharedInfrastructureService.RunInfrastructureSteps(cwdInfo, blazorProjectType, csprojFile, outputProjectConfig.RootNamespace);
        }

        console.MarkupLine($"  Copied to [yellow]{Path.Join(outputProjectConfig.ComponentsOutputDir, definition.Name)}/[/]");
        console.MarkupLine($"Component `[green]{definition.Name}[/]` added successfully.");
    }

    private string BuildTargetNamespace(OutputProjectConfig config, string componentName)
    {
        var componentsDir = config.ComponentsOutputDir
            .TrimStart('.', '/', '\\')
            .Replace('/', '.')
            .Replace('\\', '.')
            .Trim('.');

        return $"{config.RootNamespace}.{componentsDir}.{componentName}";
    }

    private void UpdateNamespacesInComponent(DirectoryInfo componentDir, string targetNamespaceBase, OutputProjectConfig config)
    {
        UpdateNamespacesInDirectory(componentDir, componentDir, targetNamespaceBase, config);
    }

    private void UpdateNamespacesInDirectory(DirectoryInfo componentRoot, DirectoryInfo directory, string targetNamespaceBase, OutputProjectConfig config)
    {
        var rootNamespace = config.RootNamespace;

        foreach (var razorFile in directory.EnumerateFiles("*.razor", SearchOption.AllDirectories))
        {
            var fileTargetNamespace = GetTargetNamespaceForFile(componentRoot, razorFile.Directory!, targetNamespaceBase);
            var content = File.ReadAllText(razorFile.FullName);
            content = namespaceService.ReplaceNamespaceInRazor(content, fileTargetNamespace);
            content = usingService.ReplaceUsingsInRazor(content, "ShadcnBlazor.Components", rootNamespace + ".Components.Core");
            content = usingService.ReplaceUsingsInRazor(content, "ShadcnBlazor", rootNamespace);
            File.WriteAllText(razorFile.FullName, content);
        }

        foreach (var csFile in directory.EnumerateFiles("*.razor.cs", SearchOption.AllDirectories)
            .Concat(directory.EnumerateFiles("*.cs", SearchOption.AllDirectories)))
        {
            var fileTargetNamespace = GetTargetNamespaceForFile(componentRoot, csFile.Directory!, targetNamespaceBase);
            var content = File.ReadAllText(csFile.FullName);
            content = namespaceService.ReplaceNamespaceInCs(content, fileTargetNamespace);
            content = usingService.ReplaceUsingsInCs(content, "ShadcnBlazor.Components", rootNamespace + ".Components.Core");
            content = usingService.ReplaceUsingsInCs(content, "ShadcnBlazor", rootNamespace);
            File.WriteAllText(csFile.FullName, content);
        }
    }

    private static string GetTargetNamespaceForFile(DirectoryInfo componentRoot, DirectoryInfo fileDir, string targetNamespaceBase)
    {
        var relativePath = Path.GetRelativePath(componentRoot.FullName, fileDir.FullName);
        if (relativePath == "." || string.IsNullOrEmpty(relativePath))
            return targetNamespaceBase;
        var suffix = relativePath.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.');
        return $"{targetNamespaceBase}.{suffix}";
    }
}
