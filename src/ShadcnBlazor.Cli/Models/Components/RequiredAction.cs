namespace ShadcnBlazor.Cli.Models.Components;

/// <summary>
/// Represents an additional action that needs to be performed in the client project to fully integrate a component, such as adding a service to the DI container or adding a NuGet dependency.
/// This is separate from the dependencies between components.
/// </summary>
public abstract record RequiredAction;

/// <summary>
/// Represents an action where some service needs to be added to the client project's DI container, either with or without an interface.
/// </summary>
public sealed record AddToServicesAction : RequiredAction
{
    public enum ServiceLifetime
    {
        Transient,
        Scoped,
        Singleton
    }
    
    public ServiceLifetime Lifetime { get; }
    public string? InterfaceType { get; }
    public string ImplementationType { get; }
    
    public AddToServicesAction(ServiceLifetime lifetime, string interfaceType, string implementationType)
    {
        Lifetime = lifetime;
        InterfaceType = interfaceType;
        ImplementationType = implementationType;
    }
    
    public AddToServicesAction(string interfaceType, string implementationType)
    {
        Lifetime = ServiceLifetime.Scoped;
        InterfaceType = interfaceType;
        ImplementationType = implementationType;
    }
    
    public AddToServicesAction(string implementationType)
    {
        Lifetime = ServiceLifetime.Scoped;
        ImplementationType = implementationType;
    }
}

/// <summary>
/// Represents an action where a NuGet (.NET) package dependency needs to be added to the client project, with a specified version.
/// </summary>
public sealed record AddNugetDependencyAction(string PackageName, string Version) : RequiredAction;

/// <summary>
/// Represents an action where a js file from the component library needs to be copied to the client project.
/// Contains only the filetype name. Assumes consumer code knows where this file will be located on the component library.
/// </summary>
public sealed record CopyJsAction(string JsFileName) : RequiredAction;

/// <summary>
/// Represents an action where a css file from the component library needs to be copied to the client project.
/// Contains only the filetype name. Assumes consumer code knows where this file will be located on the component library.
/// </summary>
public sealed record CopyCssAction(string CssFileName) : RequiredAction;

/// <summary>
/// Represents an action where a js file from the component library needs to be copied to the client project.
/// </summary>
public sealed record LinkExternalJsAction(string PackageName, string Version) : RequiredAction;

/// <summary>
/// Represents an action where we merge using statements into the client's _Imports.razor file. Contains the namespaces to be merged.
/// </summary>
public sealed record MergeToImportsAction(string[] Namespaces) : RequiredAction;

/// <summary>
/// Represents an action where CSS link tags are added to the root file for each CSS file in the project's wwwroot/css.
/// </summary>
public sealed record AddCssLinksToRootAction : RequiredAction;

/// <summary>
/// Represents an action where a using statement and service call are added to Program.cs.
/// </summary>
public sealed record AddProgramServiceAction(string UsingNamespace, string ServiceCall) : RequiredAction;