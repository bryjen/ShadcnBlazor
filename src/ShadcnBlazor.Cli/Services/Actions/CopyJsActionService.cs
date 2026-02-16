using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Models.Components;
using Spectre.Console;

namespace ShadcnBlazor.Cli.Services.Actions;

public class CopyJsActionService
{
    private readonly IAnsiConsole _console;

    public CopyJsActionService(IAnsiConsole console)
    {
        _console = console;
    }

    public void Execute(CopyJsAction action, ActionContext context)
    {
        var srcFile = new FileInfo(Path.Join(context.AssemblyDir.FullName, "wwwroot", "js", action.JsFileName));
        var outDir = new DirectoryInfo(Path.Join(context.Cwd.FullName, "wwwroot", "js"));
        if (!srcFile.Exists)
            return;
        outDir.Create();
        var destFile = new FileInfo(Path.Join(outDir.FullName, action.JsFileName));
        File.Copy(srcFile.FullName, destFile.FullName, overwrite: true);
        _console.MarkupLine($"  Copied [yellow]{action.JsFileName}[/] to wwwroot/js/.");
    }
}
