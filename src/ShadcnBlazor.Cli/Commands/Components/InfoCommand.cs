using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ShadcnBlazor.Cli.Commands.Components;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class InfoCommand(
    ComponentService componentService,
    IAnsiConsole console) 
    : Command<InfoCommand.InfoSettings>
{
    public class InfoSettings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("The name of the component. (input, textarea, checkbox, etc.)")]
        public required string Name { get; init; }
    }

    public override int Execute(CommandContext context, InfoSettings settings, CancellationToken cancellation)
    {
        try
        {
            var componentsWithMetadata = componentService.LoadComponents();
            var component = componentService.FindComponent(componentsWithMetadata, settings.Name);

            // create temp directory object so we can use the build component tree function
            var tempDir = Path.GetTempPath();
            var tempOutputProjectConfig = new OutputProjectConfig
            {
                ComponentsOutputDir = Path.Join(tempDir, "components"),
                RootNamespace = "temp"
            };
            
            // print component info
            console.MarkupLine("[yellow]Component Info:[/]");
            console.MarkupLine($"[yellow]Name:[/]\t{component.ComponentMetadata.Name}");
            console.MarkupLine($"[yellow]FullName:[/]\t{component.FullName}");
            
            console.WriteLine();
            
            // print dependency tree
            var dependencyTree = ComponentDependencyTree.BuildComponentDependencyTree(
                tempOutputProjectConfig, componentsWithMetadata, component.ComponentMetadata.Name.Trim().ToLower());
            var directDependencies = dependencyTree.RootNode.ResolvedDependencies.Count;
            var dependenciesText = directDependencies == 0 
                ? "\t(No dependencies)" 
                : directDependencies == 1 
                    ? "\t(1 dependency)" 
                    : $"\t({directDependencies} dependencies)";
            console.MarkupLine($"[yellow]Component Dependency Tree:[/]{dependenciesText}");
            console.Write(dependencyTree.AsSpectreConsoleTree());
            
            return 0;
        }
        catch (CliException ex)
        {
            console.MarkupLine($"[red]{ex.Message}[/]");
            console.MarkupLine("Component addition cancelled.");
            return 1;
        }
    }
}
