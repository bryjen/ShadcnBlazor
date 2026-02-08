using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using ShadcnBlazor.Cli.Files;
using ShadcnBlazor.Cli.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using static ShadcnBlazor.Cli.PromptUtils;

namespace ShadcnBlazor.Cli.Args;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class InitCommand(
    GreetingService greetingService, 
    IAnsiConsole console) 
    : Command<InitCommand.InitSettings>
{
    public class InitSettings : CommandSettings
    {
        [CommandOption("-b|--base-color")]
        [Description("The base color to use. (neutral, gray, zinc, stone, slate)")]
        [DefaultValue("neutral")]
        public string BaseColor { get; init; } = string.Empty;
        
        [CommandOption("-c|--comp-dir")]
        [Description("The base directory to copy components to.")]
        [DefaultValue("./Components")]
        public string ComponentsOutputDir { get; init; } = string.Empty;
        
        [CommandOption("-s|--service-dir")]
        [Description("The directory to copy services to.")]
        [DefaultValue("./Services/Components")]
        public string ServicesOutputDir { get; init; } = string.Empty;
        
        [CommandOption("--silent")]
        [Description("Mutes output.")]
        [DefaultValue("false")]
        public bool Silent { get; init; } = false;
    }

    public override int Execute(CommandContext context, InitSettings settings, CancellationToken cancellation)
    {
        var cwd = Directory.GetCurrentDirectory();
        var cwdInfo = new DirectoryInfo(cwd);
        var csprojFile = CsprojUtils.GetCsproj(new DirectoryInfo(cwd));
        if (csprojFile is null)
        {
            console.MarkupLine("Couldn't find a [yellow].csproj[/] file in the current directory.");
            console.MarkupLine("Initialization cancelled.");
            return 1;
        }

        var isBlazorProject = CsprojUtils.IsBlazorProject(csprojFile.FullName);
        if (!isBlazorProject)
        {
            console.MarkupLine("The current project isn't configured as a Blazor-compatible project.");
            console.MarkupLine("Initialization cancelled.");
            return 1;
        }
        
        var projectConf = InitProjectConfig(settings, cwdInfo);
        if (projectConf is null)
        {
            console.MarkupLine("Initialization cancelled.");
            return 0;
        }

        var copyFilesResult = CopyRequiredFiles(cwdInfo);
        if (copyFilesResult != 0)
        {
            console.MarkupLine("Initialization cancelled.");
            return copyFilesResult;
        }
        
        var modifyFilesResult = ModifyExistingFiles();
        if (modifyFilesResult != 0)
        {
            console.MarkupLine("Initialization cancelled.");
            return copyFilesResult;
        }
        
        return 0;
    }

    private OutputProjectConfig? InitProjectConfig(InitSettings settings, DirectoryInfo cwdInfo)
    {
        const string configFileName = "shadcn-blazor.yaml";
        var projectConfig = new OutputProjectConfig
        {
            ComponentsOutputDir = settings.ComponentsOutputDir,
            ServicesOutputDir = settings.ServicesOutputDir
        };
        
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(projectConfig);

        var fileInfo = new FileInfo(Path.Join(cwdInfo.FullName, configFileName));
        if (fileInfo.Exists)
        {
            var response = PromptUser($"`[yellow]{configFileName}[/]` exists. Override? (`[yellow]No[/]` will cancel the initialization)");
            if (response is ConfirmationResponse.No)
                return null;
        }
        
        File.WriteAllText(fileInfo.FullName, yaml);
        return projectConfig;
    }

    private int CopyRequiredFiles(DirectoryInfo cwdInfo)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblyDir = Path.GetDirectoryName(executingAssembly.Location) ?? throw new NotImplementedException();
        var assemblyDirInfo = new DirectoryInfo(assemblyDir);
        
        var srcWwwRootCss = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "css"));
        var outWwwRootCss = new DirectoryInfo(Path.Join(cwdInfo.FullName, "wwwroot", "css"));
        CopyDirectory(srcWwwRootCss.FullName, outWwwRootCss.FullName);
        console.MarkupLine("Copied css files to [yellow]wwwroot/css[/].");
        
        // copy in & out css files
        // js files 
        return 0;
    }

    private int ModifyExistingFiles()
    {
        // imports.razor
        // services in program.cs
        // servcie components in main layout
        
        // can't add references yet because they differ between wasm and server
        return 0;
    }
    
    /// Recursively copies all contents from a directory to another
    private static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);
    
        // Copy files
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(destDir, fileName), overwrite: true);
        }
    
        // Copy subdirectories recursively
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(subDir);
            CopyDirectory(subDir, Path.Combine(destDir, dirName));
        }
    }
}
