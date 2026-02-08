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

        var blazorProjectType = CsprojUtils.GetBlazorProjectType(csprojFile.FullName);
        if (blazorProjectType is null)
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
        
        var modifyFilesResult = ModifyExistingFiles(cwdInfo, blazorProjectType.Value);
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
        
        // copy css files
        var srcCssFiles = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "css"));
        var outCssFiles = new DirectoryInfo(Path.Join(cwdInfo.FullName, "wwwroot", "css"));
        CopyDirectory(srcCssFiles.FullName, outCssFiles.FullName);
        console.MarkupLine("Copied .css files to [yellow]wwwroot/css[/].");
        
        // copy js files
        var srcJsFiles = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "js"));
        var outJsFiles = new DirectoryInfo(Path.Join(cwdInfo.FullName, "wwwroot", "js"));
        CopyDirectory(srcJsFiles.FullName, outJsFiles.FullName);
        console.MarkupLine("Copied .js files to [yellow]wwwroot/css[/].");
        
        return 0;
    }

    private int ModifyExistingFiles(DirectoryInfo cwdInfo, BlazorProjectType projectType)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblyDir = Path.GetDirectoryName(executingAssembly.Location) ?? throw new NotImplementedException();
        var assemblyDirInfo = new DirectoryInfo(assemblyDir);
        
        // _Imports.razor
        var srcImportsFile = new FileInfo(Path.Join(assemblyDirInfo.FullName, "_Imports.razor"));
        var outImportsFile = new FileInfo(Path.Join(cwdInfo.FullName, projectType.GetImportsPath()));
        if (srcImportsFile.Exists && outImportsFile.Exists)
        {
            var srcImportsContent = File.ReadAllText(srcImportsFile.FullName);
            var outImportsContent = File.ReadAllText(outImportsFile.FullName);
            var mergedImportsContent = $"{srcImportsContent}\n\n{outImportsContent}";
            File.WriteAllText(outImportsFile.FullName, mergedImportsContent);
            console.MarkupLine("Updated `[yellow]_Imports.razor[/]` file.");
        }
        
        // TODO: services in program.cs
        // TODO: servcie components in main layout
        
        // adding references to css and js files in index.html or App.razor
        var targetFile = projectType == BlazorProjectType.WebAssembly
            ? Path.Combine(cwdInfo.FullName, projectType.GetIndexHtmlPath())
            : Path.Combine(cwdInfo.FullName, projectType.GetAppRazorPath());
        var targetFileInfo = new FileInfo(targetFile);
        var srcCssFiles = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "css")).EnumerateFiles();
        var srcJsFiles = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "js")).EnumerateFiles();
        
        ModifyRootFile(targetFileInfo, srcCssFiles, srcJsFiles);
        
        return 0;
    }

    private void ModifyRootFile(FileInfo targetFileInfo, IEnumerable<FileInfo> cssFiles, IEnumerable<FileInfo> jsFiles)
    {
        if (!targetFileInfo.Exists)
        {
            console.MarkupLine($"[red]Warning: Root file not found at {targetFileInfo.FullName}[/]");
            return;
        }
    
        var content = File.ReadAllText(targetFileInfo.FullName);
        var modified = false;
    
        // Add CSS references
        foreach (var cssFile in cssFiles)
        {
            var cssLink = $"css/{cssFile.Name}";
            if (!content.Contains(cssLink))
            {
                var cssTag = $"    <link rel=\"stylesheet\" href=\"{cssLink}\" />";
                content = content.Replace("</head>", $"{cssTag}\n</head>");
                modified = true;
            }
        }
    
        // Add JS references
        foreach (var jsFile in jsFiles)
        {
            var jsScript = $"js/{jsFile.Name}";
            if (!content.Contains(jsScript))
            {
                var scriptTag = $"    <script src=\"{jsScript}\"></script>";
                content = content.Replace("</body>", $"{scriptTag}\n</body>");
                modified = true;
            }
        }
    
        if (modified)
        {
            File.WriteAllText(targetFileInfo.FullName, content);
            console.MarkupLine($"Updated root file: [yellow]{targetFileInfo.Name}[/]");
        }
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
