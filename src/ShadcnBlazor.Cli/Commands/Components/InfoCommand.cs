using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Models.Components;
using ShadcnBlazor.Services.Models;
using ShadcnBlazor.Cli.Services;
using ShadcnBlazor.Cli.Services.Components;
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
            var components = componentService.LoadComponents();
            var component = componentService.FindComponent(components, settings.Name);

            var tempDir = Path.GetTempPath();
            var tempOutputProjectConfig = new OutputProjectConfig
            {
                ComponentsOutputDir = Path.Join(tempDir, "components"),
                RootNamespace = "temp"
            };

            console.MarkupLine("[yellow]Component Info:[/]");
            console.MarkupLine($"[yellow]Name:[/]\t{component.Name}");
            console.MarkupLine($"[yellow]Description:[/]\t{component.Description}");

            console.WriteLine();

            var dependencyTree = ComponentDependencyTree.BuildComponentDependencyTree(
                tempOutputProjectConfig, components, component.Name.Trim().ToLower());
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
            console.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            console.MarkupLine("Component addition cancelled.");
            return 1;
        }
    }
}
