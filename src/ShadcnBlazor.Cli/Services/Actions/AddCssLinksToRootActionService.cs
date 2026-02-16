using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Models.Components;
using Spectre.Console;

namespace ShadcnBlazor.Cli.Services.Actions;

public class AddCssLinksToRootActionService
{
    private readonly IAnsiConsole _console;

    public AddCssLinksToRootActionService(IAnsiConsole console)
    {
        _console = console;
    }

    public void Execute(AddCssLinksToRootAction action, ActionContext context)
    {
        var cssDir = new DirectoryInfo(Path.Join(context.Cwd.FullName, "wwwroot", "css"));
        if (!cssDir.Exists)
            return;

        var targetPath = context.BlazorProjectType == BlazorProjectType.WebAssembly
            ? Path.Combine(context.Cwd.FullName, context.BlazorProjectType.GetIndexHtmlPath()!)
            : Path.Combine(context.Cwd.FullName, context.BlazorProjectType.GetAppRazorPath());
        var targetFile = new FileInfo(targetPath);
        if (!targetFile.Exists)
        {
            _console.MarkupLine($"[yellow]  Warning: Root file not found at {targetPath}[/]");
            return;
        }

        var content = File.ReadAllText(targetFile.FullName);
        var modified = false;

        foreach (var cssFile in cssDir.EnumerateFiles("*.css"))
        {
            var cssLink = $"css/{cssFile.Name}";
            if (!content.Contains(cssLink))
            {
                var cssTag = $"    <link rel=\"stylesheet\" href=\"{cssLink}\" />";
                content = content.Replace("</head>", $"{cssTag}\n</head>");
                modified = true;
            }
        }

        if (modified)
        {
            File.WriteAllText(targetFile.FullName, content);
            _console.MarkupLine($"  Updated root file: [yellow]{targetFile.Name}[/]");
        }
    }
}
