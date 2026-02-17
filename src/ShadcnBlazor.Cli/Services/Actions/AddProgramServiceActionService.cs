using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Models.Components;
using Spectre.Console;

namespace ShadcnBlazor.Cli.Services.Actions;

public class AddProgramServiceActionService
{
    private readonly IAnsiConsole _console;

    public AddProgramServiceActionService(IAnsiConsole console)
    {
        _console = console;
    }

    public void Execute(AddProgramServiceAction action, ActionContext context)
    {
        var programCsPath = Path.Combine(context.Cwd.FullName, "Program.cs");
        var programCsFile = new FileInfo(programCsPath);
        if (!programCsFile.Exists)
        {
            _console.MarkupLine("[yellow]  Warning: Program.cs not found.[/]");
            return;
        }

        var content = File.ReadAllText(programCsFile.FullName);
        var modified = false;

        var usingLine = $"using {action.UsingNamespace};";
        if (!content.Contains(action.UsingNamespace))
        {
            var lastUsingIndex = content.LastIndexOf("using ", StringComparison.Ordinal);
            if (lastUsingIndex >= 0)
            {
                var endOfLine = content.IndexOf(';', lastUsingIndex) + 1;
                var insertIndex = content.IndexOf('\n', endOfLine) + 1;
                if (insertIndex <= 0) insertIndex = endOfLine;
                content = content.Insert(insertIndex, usingLine + "\n");
                modified = true;
            }
        }

        if (!content.Contains(action.ServiceCall))
        {
            var insertPoint = GetInsertPointAfterBuilderCreation(content);
            if (insertPoint >= 0)
            {
                var serviceCall = action.ServiceCall.TrimEnd(';');
                var serviceLine = $"builder.Services.{serviceCall};";
                content = content.Insert(insertPoint, "\n    " + serviceLine);
                modified = true;
            }
        }

        if (modified)
        {
            File.WriteAllText(programCsFile.FullName, content);
            _console.MarkupLine("  Updated `[yellow]Program.cs[/]`.");
        }
    }

    private static int GetInsertPointAfterBuilderCreation(string content)
    {
        var marker = content.IndexOf("CreateBuilder(args);", StringComparison.Ordinal);
        var suffix = "CreateBuilder(args);";
        if (marker < 0)
        {
            marker = content.IndexOf("CreateDefault(args);", StringComparison.Ordinal);
            suffix = "CreateDefault(args);";
        }
        if (marker < 0)
            return -1;

        var afterSemicolon = marker + suffix.Length;
        var newlineIndex = content.IndexOf('\n', afterSemicolon);
        return newlineIndex >= 0 ? newlineIndex + 1 : afterSemicolon;
    }
}
