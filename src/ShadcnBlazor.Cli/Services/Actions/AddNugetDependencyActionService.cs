using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Services.Models;
using Spectre.Console;

namespace ShadcnBlazor.Cli.Services.Actions;

public class AddNugetDependencyActionService
{
    private readonly CsprojService _csprojService;
    private readonly IAnsiConsole _console;

    public AddNugetDependencyActionService(CsprojService csprojService, IAnsiConsole console)
    {
        _csprojService = csprojService;
        _console = console;
    }

    public void Execute(AddNugetDependencyAction action, ActionContext context)
    {
        if (_csprojService.EnsurePackageReference(context.CsprojFile, action.PackageName, action.Version))
            _console.MarkupLine($"  Added `[green]{action.PackageName}[/]` to .csproj.");
    }
}
