using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Services.Models;
using Spectre.Console;

namespace ShadcnBlazor.Cli.Services.Actions;

public class MergeToImportsActionService
{
    private readonly UsingService _usingService;
    private readonly IAnsiConsole _console;

    public MergeToImportsActionService(UsingService usingService, IAnsiConsole console)
    {
        _usingService = usingService;
        _console = console;
    }

    public void Execute(MergeToImportsAction action, ActionContext context)
    {
        var outImportsPath = Path.Combine(context.Cwd.FullName, context.BlazorProjectType.GetImportsPath());
        var outImportsFile = new FileInfo(outImportsPath);
        if (!outImportsFile.Exists)
        {
            _console.MarkupLine($"[yellow]  Warning: _Imports.razor not found at {outImportsPath}[/]");
            return;
        }

        var rootNamespace = context.RootNamespace;
        var componentsCore = rootNamespace + ".Components.Core";

        var resolvedNamespaces = action.Namespaces
            .Select(ns => ns
                .Replace("ShadcnBlazor.Components", componentsCore)
                .Replace("ShadcnBlazor", rootNamespace))
            .ToList();

        var outContent = File.ReadAllText(outImportsFile.FullName);
        var existingNamespaces = outContent
            .Split('\n')
            .Select(l => l.Trim())
            .Where(l => l.StartsWith("@using "))
            .Select(l => l["@using ".Length..].TrimEnd(';', ' '))
            .ToHashSet();

        var newLines = resolvedNamespaces
            .Where(ns => !existingNamespaces.Contains(ns))
            .Select(ns => $"@using {ns}")
            .ToList();

        if (newLines.Count == 0)
            return;

        var toPrepend = string.Join("\n", newLines) + "\n\n";
        var merged = toPrepend + outContent;
        File.WriteAllText(outImportsFile.FullName, merged);
        _console.MarkupLine("  Updated `[yellow]_Imports.razor[/]`.");
    }
}
