using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Services.Models;
using Spectre.Console;

namespace ShadcnBlazor.Cli.Services.Actions;

public class CopyCssActionService
{
    private readonly FileSystemService _fileSystemService;
    private readonly IAnsiConsole _console;

    public CopyCssActionService(FileSystemService fileSystemService, IAnsiConsole console)
    {
        _fileSystemService = fileSystemService;
        _console = console;
    }

    public void Execute(CopyCssAction action, ActionContext context)
    {
        var srcFile = new FileInfo(Path.Join(context.AssemblyDir.FullName, "wwwroot", "css", action.CssFileName));
        var outDir = new DirectoryInfo(Path.Join(context.Cwd.FullName, "wwwroot", "css"));
        if (!srcFile.Exists)
            return;
        outDir.Create();
        var destFile = new FileInfo(Path.Join(outDir.FullName, action.CssFileName));
        File.Copy(srcFile.FullName, destFile.FullName, overwrite: true);
        _console.MarkupLine($"  Copied [yellow]{action.CssFileName}[/] to wwwroot/css/.");
    }
}
