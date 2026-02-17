using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Models.Components;
using ShadcnBlazor.Cli.Services;
using ShadcnBlazor.Cli.Services.Actions;
using ShadcnBlazor.Cli.Services.Components;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ShadcnBlazor.Cli.Commands.Components;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class AddCommand(
    FileSystemService fileSystemService,
    ProjectValidator projectValidator,
    ProjectNamespaceService projectNamespaceService,
    ComponentService componentService,
    CopyCssActionService copyCssActionService,
    CopyJsActionService copyJsActionService,
    AddCssLinksToRootActionService addCssLinksToRootActionService,
    AddNugetDependencyActionService addNugetDependencyActionService,
    AddProgramServiceActionService addProgramServiceActionService,
    MergeToImportsActionService mergeToImportsActionService,
    AddToServicesActionService addToServicesActionService,
    NamespaceService namespaceService,
    UsingService usingService,
    IAnsiConsole console)
    : Command<AddCommand.AddSettings>
{
    private const string ComponentsOutputDir = "./Components/Core";

    public class AddSettings : CommandSettings
    {
        [CommandArgument(0, "[name]")]
        [Description("The name of the component to add. (input, textarea, checkbox, etc.) Omit when using --all.")]
        public string? Name { get; init; }

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
            if (!settings.AddAllComponents && string.IsNullOrWhiteSpace(settings.Name))
            {
                throw new CliException("Component name is required. Specify a component name or use --all to add all components.");
            }

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
            var srcDirInfo = componentService.GetComponentsSourceDirectory();

            if (settings.AddAllComponents)
            {
                var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var failed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var component in components)
                {
                    try
                    {
                        var dependencyTree = ComponentDependencyTree.BuildComponentDependencyTree(
                            outputProjectConfig, components, component.Name.Trim().ToLower());

                        if (HasFailedDependency(dependencyTree.RootNode, failed))
                        {
                            var failedDep = GetFirstFailedDependency(dependencyTree.RootNode, failed)!;
                            if (!settings.Silent)
                                console.MarkupLine($"[yellow]Skipping {component.Name} (depends on failed {failedDep}).[/]");
                            failed.Add(component.Name);
                            continue;
                        }

                        var effectiveOverwrite = settings.Overwrite || settings.AddAllComponents;
                        AddComponentWithDependencies(outputProjectConfig, cwdInfo, srcDirInfo, dependencyTree, blazorProjectType, csprojFile, settings.Silent, effectiveOverwrite, added);
                    }
                    catch (ComponentSourceNotFoundException ex)
                    {
                        failed.Add(ex.ComponentName);
                        failed.Add(component.Name);
                        if (!settings.Silent)
                            console.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
                    }
                    catch (CliException ex)
                    {
                        failed.Add(component.Name);
                        if (!settings.Silent)
                            console.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
                    }
                    catch (System.Exception ex)
                    {
                        failed.Add(component.Name);
                        if (!settings.Silent)
                            console.MarkupLine($"[red]Failed to add {component.Name}: {Markup.Escape(ex.Message)}[/]");
                    }
                }

                if (failed.Count > 0)
                {
                    if (!settings.Silent)
                        console.MarkupLine($"[yellow]Component addition completed with {failed.Count} failure(s): {string.Join(", ", failed.OrderBy(x => x))}[/]");
                    return 1;
                }
            }
            else
            {
                var componentToAdd = componentService.FindComponent(components, settings.Name!);
                var dependencyTree = ComponentDependencyTree.BuildComponentDependencyTree(
                    outputProjectConfig, components, componentToAdd.Name.Trim().ToLower());
                AddComponentWithDependencies(outputProjectConfig, cwdInfo, srcDirInfo, dependencyTree, blazorProjectType, csprojFile, settings.Silent, settings.Overwrite);
            }

            return 0;
        }
        catch (CliException ex)
        {
            console.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
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
        bool overwrite = false,
        HashSet<string>? sharedAdded = null)
    {
        var added = sharedAdded ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void AddComponentWithDependenciesCore(ComponentDependencyNode componentDependencyNode)
        {
            foreach (var dependency in componentDependencyNode.ResolvedDependencies)
            {
                AddComponentWithDependenciesCore(dependency);
            }

            if (added.Contains(componentDependencyNode.Component.Name))
                return;

            AddComponent(outputProjectConfig, cwdInfo, srcDirInfo, componentDependencyNode.Component, blazorProjectType, csprojFile, silent, overwrite);
            added.Add(componentDependencyNode.Component.Name);
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

        var assemblyDir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
        var actionContext = new ActionContext(cwdInfo, blazorProjectType, csprojFile, outputProjectConfig.RootNamespace, assemblyDir);
        ExecuteActions(definition.RequiredActions, actionContext);

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

    private void ExecuteActions(RequiredAction[] actions, ActionContext context)
    {
        foreach (var action in actions)
        {
            switch (action)
            {
                case CopyCssAction a: copyCssActionService.Execute(a, context); break;
                case CopyJsAction a: copyJsActionService.Execute(a, context); break;
                case AddCssLinksToRootAction a: addCssLinksToRootActionService.Execute(a, context); break;
                case AddNugetDependencyAction a: addNugetDependencyActionService.Execute(a, context); break;
                case AddProgramServiceAction a: addProgramServiceActionService.Execute(a, context); break;
                case MergeToImportsAction a: mergeToImportsActionService.Execute(a, context); break;
                case AddToServicesAction a: addToServicesActionService.Execute(a, context); break;
            }
        }
    }

    private static bool HasFailedDependency(ComponentDependencyNode node, HashSet<string> failed)
    {
        if (failed.Contains(node.Component.Name))
            return true;
        return node.ResolvedDependencies.Any(d => HasFailedDependency(d, failed));
    }

    private static string? GetFirstFailedDependency(ComponentDependencyNode node, HashSet<string> failed)
    {
        if (failed.Contains(node.Component.Name))
            return node.Component.Name;
        foreach (var dep in node.ResolvedDependencies)
        {
            var found = GetFirstFailedDependency(dep, failed);
            if (found != null)
                return found;
        }
        return null;
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
