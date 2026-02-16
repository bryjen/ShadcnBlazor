using System.Reflection;
using ShadcnBlazor.Cli.Models;
using Spectre.Console;

namespace ShadcnBlazor.Cli.Services;

public class FirstTimeSetupService
{
    private const string TailwindMergePackageName = "TailwindMerge.NET";
    private const string TailwindMergePackageVersion = "1.2.0";

    private readonly FileSystemService _fileSystemService;
    private readonly CsprojService _csprojService;
    private readonly NamespaceService _namespaceService;
    private readonly UsingService _usingService;
    private readonly IAnsiConsole _console;

    public FirstTimeSetupService(
        FileSystemService fileSystemService,
        CsprojService csprojService,
        NamespaceService namespaceService,
        UsingService usingService,
        IAnsiConsole console)
    {
        _fileSystemService = fileSystemService;
        _csprojService = csprojService;
        _namespaceService = namespaceService;
        _usingService = usingService;
        _console = console;
    }

    public void RunFirstTimeSetup(
        string rootNamespace,
        DirectoryInfo cwdInfo,
        BlazorProjectType blazorProjectType,
        FileInfo csprojFile,
        bool overwriteShared = false)
    {
        var assemblyDir = GetAssemblyDirectory();
        var assemblyDirInfo = new DirectoryInfo(assemblyDir);

        CopyCssAndJsAssets(cwdInfo, assemblyDirInfo);
        CopySharedWithNamespaceRewrite(cwdInfo, assemblyDirInfo, rootNamespace, overwriteShared);
        UpdateImportsAndRootFile(cwdInfo, blazorProjectType, rootNamespace, assemblyDirInfo);
        EnsureTailwindMergePackage(csprojFile);
        EnsureProgramCsServices(cwdInfo);

        _console.MarkupLine("[green]First-time setup completed successfully.[/]");
    }

    private static string GetAssemblyDirectory()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        return Path.GetDirectoryName(executingAssembly.Location)
            ?? throw new InvalidOperationException("Could not determine assembly directory.");
    }

    private void CopyCssAndJsAssets(DirectoryInfo cwdInfo, DirectoryInfo assemblyDirInfo)
    {
        var srcCssDir = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "css"));
        var outCssDir = new DirectoryInfo(Path.Join(cwdInfo.FullName, "wwwroot", "css"));
        if (srcCssDir.Exists)
        {
            _fileSystemService.CopyDirectory(srcCssDir.FullName, outCssDir.FullName);
            _console.MarkupLine("Copied .css files to [yellow]wwwroot/css[/].");
        }

        var srcJsDir = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "js"));
        var outJsDir = new DirectoryInfo(Path.Join(cwdInfo.FullName, "wwwroot", "js"));
        if (srcJsDir.Exists)
        {
            _fileSystemService.CopyDirectory(srcJsDir.FullName, outJsDir.FullName);
            _console.MarkupLine("Copied .js files to [yellow]wwwroot/js[/].");
        }
    }

    private void CopySharedWithNamespaceRewrite(
        DirectoryInfo cwdInfo,
        DirectoryInfo assemblyDirInfo,
        string rootNamespace,
        bool overwriteShared)
    {
        var sourceSharedDir = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "Shared"));
        var targetSharedDir = new DirectoryInfo(Path.Join(cwdInfo.FullName, "Shared"));

        if (!sourceSharedDir.Exists)
            return;

        if (overwriteShared && targetSharedDir.Exists)
            targetSharedDir.Delete(recursive: true);

        if (!targetSharedDir.Exists || overwriteShared)
            _fileSystemService.CopyDirectory(sourceSharedDir.FullName, targetSharedDir.FullName);

        var targetNamespace = $"{rootNamespace}.Shared";
        var config = new OutputProjectConfig { RootNamespace = rootNamespace };
        UpdateNamespacesInDirectory(targetSharedDir, targetNamespace, config);

        _console.MarkupLine($"Added `[green]Shared[/]` folder.");
    }

    private void UpdateImportsAndRootFile(
        DirectoryInfo cwdInfo,
        BlazorProjectType projectType,
        string rootNamespace,
        DirectoryInfo assemblyDirInfo)
    {
        var srcImportsFile = new FileInfo(Path.Join(assemblyDirInfo.FullName, "_Imports.razor"));
        var outImportsFile = new FileInfo(Path.Join(cwdInfo.FullName, projectType.GetImportsPath()));
        if (srcImportsFile.Exists && outImportsFile.Exists)
        {
            const string sourceNamespacePrefix = "ShadcnBlazor";
            var srcImportsContent = File.ReadAllText(srcImportsFile.FullName);
            srcImportsContent = _usingService.ReplaceUsingsInRazor(srcImportsContent, sourceNamespacePrefix, rootNamespace);

            var outImportsContent = File.ReadAllText(outImportsFile.FullName);
            var mergedImportsContent = $"{srcImportsContent}\n\n{outImportsContent}";
            File.WriteAllText(outImportsFile.FullName, mergedImportsContent);
            _console.MarkupLine("Updated `[yellow]_Imports.razor[/]` file.");
        }

        var targetFile = projectType == BlazorProjectType.WebAssembly
            ? Path.Combine(cwdInfo.FullName, projectType.GetIndexHtmlPath()!)
            : Path.Combine(cwdInfo.FullName, projectType.GetAppRazorPath());
        var targetFileInfo = new FileInfo(targetFile);
        var srcCssFiles = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "css"));
        var srcJsFiles = new DirectoryInfo(Path.Join(assemblyDirInfo.FullName, "wwwroot", "js"));
        var cssFiles = srcCssFiles.Exists ? srcCssFiles.EnumerateFiles() : [];
        var jsFiles = srcJsFiles.Exists ? srcJsFiles.EnumerateFiles() : [];
        ModifyRootFile(targetFileInfo, cssFiles, jsFiles);
    }

    private void ModifyRootFile(FileInfo targetFileInfo, IEnumerable<FileInfo> cssFiles, IEnumerable<FileInfo> jsFiles)
    {
        if (!targetFileInfo.Exists)
        {
            _console.MarkupLine($"[red]Warning: Root file not found at {targetFileInfo.FullName}[/]");
            return;
        }

        var content = File.ReadAllText(targetFileInfo.FullName);
        var modified = false;

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
            _console.MarkupLine($"Updated root file: [yellow]{targetFileInfo.Name}[/]");
        }
    }

    private void EnsureTailwindMergePackage(FileInfo csprojFile)
    {
        if (_csprojService.EnsurePackageReference(csprojFile, TailwindMergePackageName, TailwindMergePackageVersion))
        {
            _console.MarkupLine($"Added `[green]{TailwindMergePackageName}[/]` package reference (v{TailwindMergePackageVersion}) to project file.");
        }
    }

    private void EnsureProgramCsServices(DirectoryInfo cwdInfo)
    {
        var programCsPath = Path.Combine(cwdInfo.FullName, "Program.cs");
        var programCsFile = new FileInfo(programCsPath);
        if (!programCsFile.Exists)
        {
            _console.MarkupLine("[yellow]Warning: Program.cs not found; skipping AddTailwindMerge registration.[/]");
            return;
        }

        var content = File.ReadAllText(programCsFile.FullName);
        var modified = false;

        const string usingTailwindMerge = "using TailwindMerge.Extensions;";
        if (!content.Contains("TailwindMerge.Extensions"))
        {
            var lastUsingIndex = content.LastIndexOf("using ", StringComparison.Ordinal);
            if (lastUsingIndex >= 0)
            {
                var endOfLine = content.IndexOf(';', lastUsingIndex) + 1;
                var insertIndex = content.IndexOf('\n', endOfLine) + 1;
                if (insertIndex <= 0) insertIndex = endOfLine;
                content = content.Insert(insertIndex, usingTailwindMerge + "\n");
                modified = true;
            }
        }

        if (!content.Contains("AddTailwindMerge"))
        {
            var insertPoint = content.IndexOf("await builder.Build()", StringComparison.Ordinal);
            if (insertPoint < 0)
                insertPoint = content.IndexOf("builder.Build()", StringComparison.Ordinal);
            if (insertPoint >= 0)
            {
                const string serviceLine = "builder.Services.AddTailwindMerge();";
                content = content.Insert(insertPoint, "    " + serviceLine + "\n    ");
                modified = true;
            }
        }

        if (modified)
        {
            File.WriteAllText(programCsFile.FullName, content);
            _console.MarkupLine("Updated `[yellow]Program.cs[/]` with AddTailwindMerge registration.");
        }
    }

    private void UpdateNamespacesInDirectory(DirectoryInfo directory, string targetNamespace, OutputProjectConfig config)
    {
        const string sourceNamespacePrefix = "ShadcnBlazor";
        var targetNamespacePrefix = config.RootNamespace;

        foreach (var razorFile in directory.EnumerateFiles("*.razor", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(razorFile.FullName);
            content = _namespaceService.ReplaceNamespaceInRazor(content, targetNamespace);
            content = _usingService.ReplaceUsingsInRazor(content, sourceNamespacePrefix, targetNamespacePrefix);
            File.WriteAllText(razorFile.FullName, content);
        }

        foreach (var csFile in directory.EnumerateFiles("*.razor.cs", SearchOption.AllDirectories)
            .Concat(directory.EnumerateFiles("*.cs", SearchOption.AllDirectories)))
        {
            var content = File.ReadAllText(csFile.FullName);
            content = _namespaceService.ReplaceNamespaceInCs(content, targetNamespace);
            content = _usingService.ReplaceUsingsInCs(content, sourceNamespacePrefix, targetNamespacePrefix);
            File.WriteAllText(csFile.FullName, content);
        }
    }
}
