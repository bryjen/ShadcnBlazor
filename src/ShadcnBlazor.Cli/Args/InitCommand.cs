using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static ShadcnBlazor.Cli.PromptUtils;

#if DEBUG
#pragma warning disable CS9113 // Parameter is unread.
#endif

namespace ShadcnBlazor.Cli.Args;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class InitCommand(
    FileSystemService fileSystemService,
    ConfigService configService,
    ProjectValidator projectValidator,
    ProjectNamespaceService projectNamespaceService,
    CsprojService csprojService,
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
        try
        {
            var cwd = Directory.GetCurrentDirectory();
            var cwdInfo = new DirectoryInfo(cwd);
            
            var csprojFile = projectValidator.ValidateAndGetCsproj(cwdInfo);
            var blazorProjectType = projectValidator.ValidateBlazorProject(csprojFile);
            
            var projectConf = InitProjectConfig(settings, cwdInfo, csprojFile);
            if (projectConf is null)
            {
                console.MarkupLine("Initialization cancelled.");
                return 0;
            }

            CopyRequiredFiles(cwdInfo);
            ModifyExistingFiles(cwdInfo, blazorProjectType);
            EnsureTwMergePackage(csprojFile);
            
            return 0;
        }
        catch (CliException ex)
        {
            console.MarkupLine($"[red]{ex.Message}[/]");
            console.MarkupLine("Initialization cancelled.");
            return 1;
        }
    }

    private OutputProjectConfig? InitProjectConfig(InitSettings settings, DirectoryInfo cwdInfo, FileInfo csprojFile)
    {
        const string configFileName = "shadcn-blazor.yaml";
        var rootNamespace = projectNamespaceService.GetRootNamespace(csprojFile);
        var projectConfig = new OutputProjectConfig
        {
            ComponentsOutputDir = settings.ComponentsOutputDir,
            ServicesOutputDir = settings.ServicesOutputDir,
            RootNamespace = rootNamespace
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

    private void CopyRequiredFiles(DirectoryInfo cwdInfo)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblyDir = Path.GetDirectoryName(executingAssembly.Location) 
            ?? throw new InvalidOperationException("Could not determine assembly directory.");
        var assemblyDirInfo = new DirectoryInfo(assemblyDir);
        
        // copy css files
        var srcCssFiles = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "css"));
        var outCssFiles = new DirectoryInfo(Path.Join(cwdInfo.FullName, "wwwroot", "css"));
        fileSystemService.CopyDirectory(srcCssFiles.FullName, outCssFiles.FullName);
        console.MarkupLine("Copied .css files to [yellow]wwwroot/css[/].");
        
        // copy js files
        var srcJsFiles = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "js"));
        var outJsFiles = new DirectoryInfo(Path.Join(cwdInfo.FullName, "wwwroot", "js"));
        fileSystemService.CopyDirectory(srcJsFiles.FullName, outJsFiles.FullName);
        console.MarkupLine("Copied .js files to [yellow]wwwroot/css[/].");
    }

    private void ModifyExistingFiles(DirectoryInfo cwdInfo, BlazorProjectType projectType)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblyDir = Path.GetDirectoryName(executingAssembly.Location) 
            ?? throw new InvalidOperationException("Could not determine assembly directory.");
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
    
    private void EnsureTwMergePackage(FileInfo csprojFile)
    {
        const string packageName = "TwMerge";
        const string packageVersion = "1.0.7";
        
        if (csprojService.EnsurePackageReference(csprojFile, packageName, packageVersion))
        {
            console.MarkupLine($"Added `[green]{packageName}[/]` package reference (v{packageVersion}) to project file.");
        }
    }
}
