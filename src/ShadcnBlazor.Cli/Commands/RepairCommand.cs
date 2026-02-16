using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ShadcnBlazor.Cli.Commands;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class RepairCommand(
    ProjectValidator projectValidator,
    ProjectNamespaceService projectNamespaceService,
    FirstTimeSetupService firstTimeSetupService,
    IAnsiConsole console)
    : Command<RepairCommand.RepairSettings>
{
    public class RepairSettings : CommandSettings
    {
        [CommandOption("--silent")]
        [Description("Mutes output.")]
        [DefaultValue("false")]
        public bool Silent { get; init; } = false;
    }

    public override int Execute(CommandContext context, RepairSettings settings, CancellationToken cancellation)
    {
        try
        {
            var cwd = Directory.GetCurrentDirectory();
            var cwdInfo = new DirectoryInfo(cwd);

            var csprojFile = projectValidator.ValidateAndGetCsproj(cwdInfo);
            var blazorProjectType = projectValidator.ValidateBlazorProject(csprojFile);
            var rootNamespace = projectNamespaceService.GetRootNamespace(csprojFile);

            firstTimeSetupService.RunFirstTimeSetup(rootNamespace, cwdInfo, blazorProjectType, csprojFile, overwriteShared: true);

            return 0;
        }
        catch (CliException ex)
        {
            console.MarkupLine($"[red]{ex.Message}[/]");
            console.MarkupLine("Repair cancelled.");
            return 1;
        }
    }
}
