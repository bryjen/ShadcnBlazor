namespace ShadcnBlazor.Cli.Services;

public enum SetupStatus
{
    Complete,
    Partial,
    Missing
}

public class SetupDetectionService
{
    private const string TailwindMergePackageName = "TailwindMerge.NET";
    private const string AddTailwindMergeMarker = "AddTailwindMerge";

    private readonly CsprojService _csprojService;

    public SetupDetectionService(CsprojService csprojService)
    {
        _csprojService = csprojService;
    }

    public bool CheckIfFirstTimeSetup(DirectoryInfo cwd, FileInfo csproj)
    {
        var hasShared = new DirectoryInfo(Path.Join(cwd.FullName, "Shared")).Exists;
        var hasTailwindMerge = _csprojService.HasPackageReference(csproj, TailwindMergePackageName);
        var hasProgramCsRegistration = CheckProgramCsHasAddTailwindMerge(cwd);

        return !hasShared || !hasTailwindMerge || !hasProgramCsRegistration;
    }

    public SetupStatus CheckSetupIntegrity(DirectoryInfo cwd, FileInfo csproj)
    {
        var hasShared = new DirectoryInfo(Path.Join(cwd.FullName, "Shared")).Exists;
        var hasTailwindMerge = _csprojService.HasPackageReference(csproj, TailwindMergePackageName);
        var hasProgramCsRegistration = CheckProgramCsHasAddTailwindMerge(cwd);

        var presentCount = (hasShared ? 1 : 0) + (hasTailwindMerge ? 1 : 0) + (hasProgramCsRegistration ? 1 : 0);

        return presentCount switch
        {
            3 => SetupStatus.Complete,
            0 => SetupStatus.Missing,
            _ => SetupStatus.Partial
        };
    }

    private static bool CheckProgramCsHasAddTailwindMerge(DirectoryInfo cwd)
    {
        var programCsPath = Path.Combine(cwd.FullName, "Program.cs");
        var programCsFile = new FileInfo(programCsPath);
        if (!programCsFile.Exists)
            return false;

        var content = File.ReadAllText(programCsFile.FullName);
        return content.Contains(AddTailwindMergeMarker);
    }
}
