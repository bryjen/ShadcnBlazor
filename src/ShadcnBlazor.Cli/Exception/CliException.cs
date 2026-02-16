namespace ShadcnBlazor.Cli.Exception;

public class CliException : System.Exception
{
    public CliException(string message) : base(message) { }
    public CliException(string message, System.Exception innerException) : base(message, innerException) { }
}

public class ProjectFileNotFoundException() 
    : CliException("Couldn't find a .csproj file in the current directory.");

public class InvalidBlazorProjectException()
    : CliException("The current project isn't configured as a Blazor-compatible project.");

public class ComponentNotFoundException(string componentName)
    : CliException($"Couldn't find the component `{componentName}`.");

public class ComponentAlreadyExistsException(string componentName)
    : CliException($"Component `{componentName}` already exists at the destination.");

public class ComponentSourceNotFoundException(string componentName)
    : CliException($"Source files for component `{componentName}` not found.");

public class OutputDirectoryExistsException(string path)
    : CliException($"Output directory `{path}` already exists.");
