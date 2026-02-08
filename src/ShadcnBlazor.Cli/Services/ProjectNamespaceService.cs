namespace ShadcnBlazor.Cli.Services;

public class ProjectNamespaceService
{
    private readonly CsprojService _csprojService;
    
    public ProjectNamespaceService(CsprojService csprojService)
    {
        _csprojService = csprojService;
    }
    
    public string GetRootNamespace(FileInfo csprojFile)
    {
        return _csprojService.GetRootNamespace(csprojFile);
    }
}
