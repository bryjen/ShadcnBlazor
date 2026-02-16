using System.Diagnostics.CodeAnalysis;
using ShadcnBlazor.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ShadcnBlazor.Cli.Commands.Components;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ListCommand(
    ComponentService componentService,
    IAnsiConsole console) 
    : Command<ListCommand.ListSettings>
{
    public class ListSettings : CommandSettings;

    public override int Execute(CommandContext context, ListSettings settings, CancellationToken cancellation)
    {
        var components = componentService.LoadComponents();
        var asStrings = components.Select((c, i) => $"[green]{i}.[/] {c.Name}");
        var asString = string.Join("\n", asStrings);
        console.MarkupLine($"Available Components: {asString}");
        return 0;
    }
}
