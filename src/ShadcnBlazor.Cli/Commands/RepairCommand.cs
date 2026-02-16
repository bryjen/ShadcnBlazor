using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ShadcnBlazor.Cli.Commands;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class RepairCommand(
    ProjectValidator projectValidator,
    ProjectNamespaceService projectNamespaceService,
    ComponentService componentService,
    SharedInfrastructureService sharedInfrastructureService,
    FileSystemService fileSystemService,
    NamespaceService namespaceService,
    UsingService usingService,
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

            var outputProjectConfig = new OutputProjectConfig
            {
                ComponentsOutputDir = "./Components/Core",
                RootNamespace = rootNamespace
            };

            var srcDirInfo = componentService.GetComponentsSourceDirectory();

            var destinationDir = new DirectoryInfo(Path.Join(cwdInfo.FullName, outputProjectConfig.ComponentsOutputDir, "Shared"));
            if (destinationDir.Exists)
                destinationDir.Delete(recursive: true);

            if (!settings.Silent)
                console.MarkupLine("Adding [green]Shared[/] (repair)...");

            fileSystemService.CopyDirectory(
                Path.Join(srcDirInfo.FullName, "Shared"),
                destinationDir.FullName);

            var targetNamespace = $"{rootNamespace}.Components.Core.Shared";
            UpdateNamespacesInComponent(destinationDir, targetNamespace, outputProjectConfig);

            sharedInfrastructureService.RunInfrastructureSteps(cwdInfo, blazorProjectType, csprojFile, rootNamespace);

            if (!settings.Silent)
            {
                console.MarkupLine($"  Copied to [yellow]{outputProjectConfig.ComponentsOutputDir}/Shared/[/]");
                console.MarkupLine("[green]Repair completed successfully.[/]");
            }

            return 0;
        }
        catch (CliException ex)
        {
            console.MarkupLine($"[red]{ex.Message}[/]");
            console.MarkupLine("Repair cancelled.");
            return 1;
        }
    }

    private void UpdateNamespacesInComponent(DirectoryInfo componentDir, string targetNamespaceBase, OutputProjectConfig config)
    {
        UpdateNamespacesInDirectory(componentDir, componentDir, targetNamespaceBase, config);
    }

    private void UpdateNamespacesInDirectory(DirectoryInfo componentRoot, DirectoryInfo directory, string targetNamespaceBase, OutputProjectConfig config)
    {
        var rootNamespace = config.RootNamespace;

        foreach (var razorFile in directory.EnumerateFiles("*.razor", SearchOption.AllDirectories))
        {
            var fileTargetNamespace = GetTargetNamespaceForFile(componentRoot, razorFile.Directory!, targetNamespaceBase);
            var content = File.ReadAllText(razorFile.FullName);
            content = namespaceService.ReplaceNamespaceInRazor(content, fileTargetNamespace);
            content = usingService.ReplaceUsingsInRazor(content, "ShadcnBlazor.Components", rootNamespace + ".Components.Core");
            content = usingService.ReplaceUsingsInRazor(content, "ShadcnBlazor", rootNamespace);
            File.WriteAllText(razorFile.FullName, content);
        }

        foreach (var csFile in directory.EnumerateFiles("*.razor.cs", SearchOption.AllDirectories)
            .Concat(directory.EnumerateFiles("*.cs", SearchOption.AllDirectories)))
        {
            var fileTargetNamespace = GetTargetNamespaceForFile(componentRoot, csFile.Directory!, targetNamespaceBase);
            var content = File.ReadAllText(csFile.FullName);
            content = namespaceService.ReplaceNamespaceInCs(content, fileTargetNamespace);
            content = usingService.ReplaceUsingsInCs(content, "ShadcnBlazor.Components", rootNamespace + ".Components.Core");
            content = usingService.ReplaceUsingsInCs(content, "ShadcnBlazor", rootNamespace);
            File.WriteAllText(csFile.FullName, content);
        }
    }

    private static string GetTargetNamespaceForFile(DirectoryInfo componentRoot, DirectoryInfo fileDir, string targetNamespaceBase)
    {
        var relativePath = Path.GetRelativePath(componentRoot.FullName, fileDir.FullName);
        if (relativePath == "." || string.IsNullOrEmpty(relativePath))
            return targetNamespaceBase;
        var suffix = relativePath.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.');
        return $"{targetNamespaceBase}.{suffix}";
    }
}
