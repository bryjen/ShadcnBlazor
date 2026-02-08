using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using ShadcnBlazor.Cli.Files;
using ShadcnBlazor.Cli.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using static ShadcnBlazor.Cli.PromptUtils;

namespace ShadcnBlazor.Cli.Args;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class AddCommand(
    GreetingService greetingService, 
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
        const string configFileName = "shadcn-blazor.yaml";
        
        var cwd = Directory.GetCurrentDirectory();
        var cwdInfo = new DirectoryInfo(cwd);
        
        // config file shit
        var configFileInfo = cwdInfo.EnumerateFiles().FirstOrDefault(f => f.Name == configFileName);
        if (configFileInfo is null)
        {
            console.MarkupLine($"Couldn't find a `[yellow]{configFileName}[/]` file in the current directory.");
            console.MarkupLine("Component addition cancelled.");
            return 1;
        }
        
        var configFileContents = File.ReadAllText(configFileInfo.FullName);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var outputProjectConfig = deserializer.Deserialize<OutputProjectConfig>(configFileContents);
        Console.WriteLine(JsonSerializer.Serialize(outputProjectConfig, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        }));
        
        // csproj shit
        var csprojFile = CsprojUtils.GetCsproj(new DirectoryInfo(cwd));
        if (csprojFile is null)
        {
            console.MarkupLine("Couldn't find a [yellow].csproj[/] file in the current directory.");
            console.MarkupLine("Component addition cancelled.");
            return 1;
        }

        var blazorProjectType = CsprojUtils.GetBlazorProjectType(csprojFile.FullName);
        if (blazorProjectType is null)
        {
            console.MarkupLine("The current project isn't configured as a Blazor-compatible project.");
            console.MarkupLine("Component addition cancelled.");
            return 1;
        }
        
        // component shit
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblyDir = Path.GetDirectoryName(executingAssembly.Location) ?? throw new NotImplementedException();
        var componentsAssemblyPath = Path.Join(assemblyDir, "ShadcnBlazor.dll");
        var assembly = Assembly.LoadFrom(componentsAssemblyPath);
        var componentsWithMetadata = ComponentData.GetComponents(assembly).ToList();
        
        var componentToAdd = componentsWithMetadata.FirstOrDefault(c =>
            string.Equals(settings.Name.Trim(), c.ComponentMetadata.Name, StringComparison.InvariantCultureIgnoreCase));
        if (componentToAdd is null)
        {
            console.MarkupLine($"Couldn't find the component `[yellow]{settings.Name}[/]`.");
            console.MarkupLine("Component addition cancelled.");
            return 1;
        }

        var dependencyTree = ComponentDependencyTree.BuildComponentDependencyTree(
            outputProjectConfig, componentsWithMetadata, settings.Name.Trim().ToLower());
        var exitCode = AddComponentWithDependencies(
            outputProjectConfig, cwdInfo, new DirectoryInfo(Path.Join(assemblyDir, "components")), dependencyTree);
        if (exitCode != 0)
        {
            console.MarkupLine("Component addition failed.");
            return exitCode;
        }
        
        return 0;
    }

    private int AddComponentWithDependencies(
        OutputProjectConfig outputProjectConfig,
        DirectoryInfo cwdInfo, 
        DirectoryInfo srcDirInfo, 
        ComponentDependencyTree componentDependencyTree)
    {
        int AddComponentWithDependenciesCore(ComponentDependencyNode componentDependencyNode)
        {
            var exitCodes = componentDependencyNode.ResolvedDependencies
                .Select(AddComponentWithDependenciesCore)
                .ToList();
            
            if (exitCodes.Any(c => c != 0))
            {
                console.MarkupLine($"Failed to add dependencies for component `[yellow]{componentDependencyNode.Component.ComponentMetadata.Name}[/]`.");
                return 1;
            }

            var exitCoe = AddComponent(outputProjectConfig, cwdInfo, srcDirInfo, componentDependencyNode.Component);
            if (exitCoe != 0)
            {
                console.MarkupLine($"Failed to add component `[yellow]{componentDependencyNode.Component.ComponentMetadata.Name}[/]`.");
                return 1;
            }

            return 0;
        }

        AddComponentWithDependenciesCore(componentDependencyTree.RootNode);
        return 0;
    }
    
    private int AddComponent(
        OutputProjectConfig outputProjectConfig,
        DirectoryInfo cwdInfo, 
        DirectoryInfo srcDirInfo, 
        ComponentData componentData)
    {
        var destinationDir = new DirectoryInfo(Path.Join(cwdInfo.FullName, outputProjectConfig.ComponentsOutputDir, componentData.ComponentMetadata.Name));
        if (destinationDir.Exists)
        {
            console.MarkupLine($"Component `[yellow]{componentData.ComponentMetadata.Name}[/]` already exists at the destination.");
            return 1;
        }
        
        var sourceDir = new DirectoryInfo(Path.Join(srcDirInfo.FullName, componentData.ComponentMetadata.Name));
        if (!sourceDir.Exists)
        {
            console.MarkupLine($"Source files for component `[yellow]{componentData.ComponentMetadata.Name}[/]` not found.");
            return 1;
        }

        CopyDirectory(sourceDir.FullName, destinationDir.FullName);
        console.MarkupLine($"Component `[green]{componentData.ComponentMetadata.Name}[/]` added successfully.");
        return 0;
    }
    
    /// Recursively copies all contents from a directory to another
    private static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);
    
        // Copy files
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(destDir, fileName), overwrite: true);
        }
    
        // Copy subdirectories recursively
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(subDir);
            CopyDirectory(subDir, Path.Combine(destDir, dirName));
        }
    }
}
