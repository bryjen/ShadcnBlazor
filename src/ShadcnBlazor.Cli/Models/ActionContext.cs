using ShadcnBlazor.Cli.Services;

namespace ShadcnBlazor.Cli.Models;

public record ActionContext(
    DirectoryInfo Cwd,
    BlazorProjectType BlazorProjectType,
    FileInfo CsprojFile,
    string RootNamespace,
    DirectoryInfo AssemblyDir);
