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
    ConfigService configService,
    ProjectValidator projectValidator,
    ComponentService componentService,
    NamespaceService namespaceService,
    UsingService usingService,
    IAnsiConsole console) 
    : Command<AddCommand.AddSettings>
{
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
    }

    public override int Execute(CommandContext context, AddSettings settings, CancellationToken cancellation)
    {
        try
        {
            var cwd = Directory.GetCurrentDirectory();
            var cwdInfo = new DirectoryInfo(cwd);
            
            var outputProjectConfig = configService.LoadConfig(cwdInfo);
            projectValidator.ValidateBlazorProject(projectValidator.ValidateAndGetCsproj(cwdInfo));
            
            var componentsWithMetadata = componentService.LoadComponents();
            var componentToAdd = componentService.FindComponent(componentsWithMetadata, settings.Name);
            
            var dependencyTree = ComponentDependencyTree.BuildComponentDependencyTree(
                outputProjectConfig, componentsWithMetadata, componentToAdd.ComponentMetadata.Name.Trim().ToLower());
            
            var srcDirInfo = componentService.GetComponentsSourceDirectory();

            AddComponentWithDependencies(outputProjectConfig, cwdInfo, srcDirInfo, dependencyTree);
            
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
        ComponentDependencyTree componentDependencyTree)
    {
        void AddComponentWithDependenciesCore(ComponentDependencyNode componentDependencyNode)
        {
            foreach (var dependency in componentDependencyNode.ResolvedDependencies)
            {
                AddComponentWithDependenciesCore(dependency);
            }

            AddComponent(outputProjectConfig, cwdInfo, srcDirInfo, componentDependencyNode.Component);
        }

        AddComponentWithDependenciesCore(componentDependencyTree.RootNode);
    }
    
    private void AddComponent(
        OutputProjectConfig outputProjectConfig,
        DirectoryInfo cwdInfo, 
        DirectoryInfo srcDirInfo, 
        ComponentData componentData)
    {
        var destinationDir = new DirectoryInfo(Path.Join(cwdInfo.FullName, outputProjectConfig.ComponentsOutputDir, componentData.ComponentMetadata.Name));
        if (destinationDir.Exists)
        {
            throw new ComponentAlreadyExistsException(componentData.ComponentMetadata.Name);
        }
        
        var sourceDir = new DirectoryInfo(Path.Join(srcDirInfo.FullName, componentData.ComponentMetadata.Name));
        if (!sourceDir.Exists)
        {
            throw new ComponentSourceNotFoundException(componentData.ComponentMetadata.Name);
        }

        fileSystemService.CopyDirectory(sourceDir.FullName, destinationDir.FullName);
        
        // Update namespaces in copied files
        var targetNamespace = BuildTargetNamespace(outputProjectConfig, componentData.ComponentMetadata.Name);
        UpdateNamespacesInComponent(destinationDir, targetNamespace, outputProjectConfig);
        
        console.MarkupLine($"Component `[green]{componentData.ComponentMetadata.Name}[/]` added successfully.");
    }
    
    private string BuildTargetNamespace(OutputProjectConfig config, string componentName)
    {
        // Convert path-like ComponentsOutputDir to namespace segment
        // e.g., "./Components" -> "Components", "Components/Sub" -> "Components.Sub"
        var componentsDir = config.ComponentsOutputDir
            .TrimStart('.', '/', '\\')
            .Replace('/', '.')
            .Replace('\\', '.')
            .Trim('.');

        return $"{config.RootNamespace}.{componentsDir}.{componentName}";
    }

    private void UpdateNamespacesInComponent(DirectoryInfo componentDir, string targetNamespace, OutputProjectConfig config)
    {
        UpdateNamespacesInDirectory(componentDir, targetNamespace, config);
    }
    
    private void UpdateNamespacesInDirectory(DirectoryInfo directory, string targetNamespace, OutputProjectConfig config)
    {
        const string sourceNamespacePrefix = "ShadcnBlazor";
        var targetNamespacePrefix = config.RootNamespace;
        
        // Process all .razor files
        foreach (var razorFile in directory.EnumerateFiles("*.razor", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(razorFile.FullName);
            content = namespaceService.ReplaceNamespaceInRazor(content, targetNamespace);
            content = usingService.ReplaceUsingsInRazor(content, sourceNamespacePrefix, targetNamespacePrefix);
            File.WriteAllText(razorFile.FullName, content);
        }
        
        // Process all .razor.cs and .cs files
        foreach (var csFile in directory.EnumerateFiles("*.razor.cs", SearchOption.AllDirectories)
            .Concat(directory.EnumerateFiles("*.cs", SearchOption.AllDirectories)))
        {
            var content = File.ReadAllText(csFile.FullName);
            content = namespaceService.ReplaceNamespaceInCs(content, targetNamespace);
            content = usingService.ReplaceUsingsInCs(content, sourceNamespacePrefix, targetNamespacePrefix);
            File.WriteAllText(csFile.FullName, content);
        }
    }
}
