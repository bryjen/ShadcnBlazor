using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ShadcnBlazor.Cli.Commands;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class NewCommand(
    FileSystemService fileSystemService,
    IAnsiConsole console)
    : Command<NewCommand.NewSettings>
{
    private const string WasmTemplateName = "WasmStandalone";
    private const string ServerTemplateName = "ServerGlobalInteractivity";

    public class NewSettings : CommandSettings
    {
        [CommandOption("--wasm")]
        [Description("Use the WebAssembly standalone template.")]
        [DefaultValue(false)]
        public bool Wasm { get; init; }

        [CommandOption("--server")]
        [Description("Use the Blazor Server (global interactivity) template.")]
        [DefaultValue(false)]
        public bool Server { get; init; }

        [CommandOption("-p|--proj <NAME>")]
        [Description("Project name, root namespace, and default output folder.")]
        public string Proj { get; init; } = string.Empty;

        [CommandOption("-o|--out <DIR>")]
        [Description("Output directory. Default: ./{proj}")]
        public string? Out { get; init; }

        [CommandOption("--net <VERSION>")]
        [Description("Target .NET version (9 only for now).")]
        [DefaultValue(9)]
        public int Net { get; init; } = 9;
    }

    public override int Execute(CommandContext context, NewSettings settings, CancellationToken cancellation)
    {
        try
        {
            if (settings.Wasm == settings.Server)
            {
                console.MarkupLine("[red]Specify exactly one of [yellow]--wasm[/] or [yellow]--server[/].[/]");
                return 1;
            }

            if (string.IsNullOrWhiteSpace(settings.Proj))
            {
                console.MarkupLine("[red]Project name [yellow]--proj[/] is required.[/]");
                return 1;
            }

            if (settings.Net != 9)
            {
                console.MarkupLine("[red]Only .NET 9 is supported. Use [yellow]--net 9[/].[/]");
                return 1;
            }

            var cwd = Directory.GetCurrentDirectory();
            var outputBaseDir = string.IsNullOrWhiteSpace(settings.Out)
                ? cwd
                : Path.GetFullPath(settings.Out);
            var outputPath = Path.Combine(outputBaseDir, settings.Proj);

            if (Directory.Exists(outputPath))
            {
                throw new OutputDirectoryExistsException(outputPath);
            }

            var templateName = settings.Wasm ? WasmTemplateName : ServerTemplateName;
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                ?? throw new InvalidOperationException("Could not determine assembly directory.");
            var templatePath = Path.Combine(assemblyDir, "templates", "net9", templateName);

            if (!Directory.Exists(templatePath))
            {
                console.MarkupLine($"[red]Template not found at [yellow]{templatePath}[/].[/]");
                return 1;
            }

            fileSystemService.CopyTemplateDirectory(templatePath, outputPath);
            console.MarkupLine($"Copied template to [yellow]{outputPath}[/].");

            RenameProjectFile(outputPath, templateName, settings.Proj);
            ReplaceNamespacesAndUsings(outputPath, templateName, settings.Proj, settings.Proj);

            console.MarkupLine($"Project [green]{settings.Proj}[/] created successfully.");
            return 0;
        }
        catch (CliException ex)
        {
            console.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            return 1;
        }
    }

    private static void RenameProjectFile(string outputPath, string templateName, string projName)
    {
        var oldCsproj = Path.Combine(outputPath, $"{templateName}.csproj");
        var newCsproj = Path.Combine(outputPath, $"{projName}.csproj");
        if (File.Exists(oldCsproj))
        {
            File.Move(oldCsproj, newCsproj);
        }
    }

    private void ReplaceNamespacesAndUsings(string outputPath, string sourceNamespace, string targetNamespace, string projName)
    {
        var dir = new DirectoryInfo(outputPath);

        foreach (var razorFile in dir.EnumerateFiles("*.razor", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(razorFile.FullName);
            content = ReplaceTemplateReferences(content, sourceNamespace, targetNamespace, projName);
            content = content.Replace(sourceNamespace, targetNamespace);
            File.WriteAllText(razorFile.FullName, content);
        }

        foreach (var csFile in dir.EnumerateFiles("*.razor.cs", SearchOption.AllDirectories)
            .Concat(dir.EnumerateFiles("*.cs", SearchOption.AllDirectories)))
        {
            var content = File.ReadAllText(csFile.FullName);
            content = ReplaceTemplateReferences(content, sourceNamespace, targetNamespace, projName);
            content = content.Replace(sourceNamespace, targetNamespace);
            File.WriteAllText(csFile.FullName, content);
        }

        foreach (var htmlFile in dir.EnumerateFiles("*.html", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(htmlFile.FullName);
            content = ReplaceTemplateReferences(content, sourceNamespace, targetNamespace, projName);
            content = content.Replace(sourceNamespace, targetNamespace);
            File.WriteAllText(htmlFile.FullName, content);
        }

    }

    private static string ReplaceTemplateReferences(string content, string sourceNamespace, string targetNamespace, string projName)
    {
        content = content.Replace($"{sourceNamespace}.styles.css", $"{projName}.styles.css");
        content = content.Replace($"@Assets[\"{sourceNamespace}.styles.css\"]", $"@Assets[\"{projName}.styles.css\"]");
        content = content.Replace($"<title>{sourceNamespace}</title>", $"<title>{projName}</title>");
        return content;
    }
}
