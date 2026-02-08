using ShadcnBlazor.Cli.Exception;

namespace ShadcnBlazor.Cli.Services;

public class ProjectValidator
{
    private readonly CsprojService _csprojService;
    
    public ProjectValidator(CsprojService csprojService)
    {
        _csprojService = csprojService;
    }
    
    public FileInfo ValidateAndGetCsproj(DirectoryInfo workingDirectory)
    {
        var csprojFile = _csprojService.GetCsproj(workingDirectory);
        if (csprojFile is null)
        {
            throw new ProjectFileNotFoundException();
        }
        
        return csprojFile;
    }
    
    public BlazorProjectType ValidateBlazorProject(FileInfo csprojFile)
    {
        var blazorProjectType = _csprojService.GetBlazorProjectType(csprojFile.FullName);
        if (blazorProjectType is null)
        {
            throw new InvalidBlazorProjectException();
        }
        
        return blazorProjectType.Value;
    }
}
