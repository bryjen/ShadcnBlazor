using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Models.Components;
using Spectre.Console;

namespace ShadcnBlazor.Cli.Services.Actions;

public class AddToServicesActionService
{
    private readonly IAnsiConsole _console;

    public AddToServicesActionService(IAnsiConsole console)
    {
        _console = console;
    }

    public void Execute(AddToServicesAction action, ActionContext context)
    {
        var programCsPath = Path.Combine(context.Cwd.FullName, "Program.cs");
        var programCsFile = new FileInfo(programCsPath);
        if (!programCsFile.Exists)
        {
            _console.MarkupLine("[yellow]  Warning: Program.cs not found.[/]");
            return;
        }

        var rootNamespace = context.RootNamespace;
        var componentsCore = rootNamespace + ".Components.Core";

        var interfaceType = action.InterfaceType != null
            ? $"{componentsCore}.{GetNamespaceForType(action.InterfaceType)}.{action.InterfaceType}"
            : null;
        var implementationType = $"{componentsCore}.{GetNamespaceForType(action.ImplementationType)}.{action.ImplementationType}";

        var serviceCall = action.Lifetime switch
        {
            AddToServicesAction.ServiceLifetime.Transient => "AddTransient",
            AddToServicesAction.ServiceLifetime.Singleton => "AddSingleton",
            _ => "AddScoped"
        };

        var methodCall = interfaceType != null
            ? $"{serviceCall}<{interfaceType}, {implementationType}>()"
            : $"{serviceCall}<{implementationType}>()";

        var content = File.ReadAllText(programCsFile.FullName);
        if (content.Contains(implementationType))
            return;

        var insertPoint = content.IndexOf("await builder.Build()", StringComparison.Ordinal);
        if (insertPoint < 0)
            insertPoint = content.IndexOf("builder.Build()", StringComparison.Ordinal);
        if (insertPoint < 0)
            return;

        var serviceLine = $"builder.Services.{methodCall}";
        content = content.Insert(insertPoint, "    " + serviceLine + "\n    ");
        File.WriteAllText(programCsFile.FullName, content);
        _console.MarkupLine("  Updated `[yellow]Program.cs[/]` with service registration.");
    }

    private static string GetNamespaceForType(string typeName)
    {
        return typeName switch
        {
            "IDialogService" or "DialogService" => "Dialog.Services",
            "IScrollLockService" or "ScrollLockService" => "Shared.Services",
            "IPopoverService" or "PopoverService" => "Popover.Services",
            "IPopoverRegistry" or "PopoverRegistry" => "Popover.Services",
            _ => "Shared.Services"
        };
    }
}
