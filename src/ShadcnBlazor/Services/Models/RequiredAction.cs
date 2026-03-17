using System.Text.Json.Serialization;

namespace ShadcnBlazor.Services.Models;

/// <summary>
/// Represents an action to perform when integrating a component into a project.
/// </summary>
[JsonDerivedType(typeof(AddToServicesAction), typeDiscriminator: "addToServices")]
[JsonDerivedType(typeof(AddNugetDependencyAction), typeDiscriminator: "addNugetDependency")]
[JsonDerivedType(typeof(CopyJsAction), typeDiscriminator: "copyJs")]
[JsonDerivedType(typeof(CopyCssAction), typeDiscriminator: "copyCss")]
[JsonDerivedType(typeof(LinkExternalJsAction), typeDiscriminator: "linkExternalJs")]
[JsonDerivedType(typeof(MergeToImportsAction), typeDiscriminator: "mergeToImports")]
[JsonDerivedType(typeof(AddCssLinksToRootAction), typeDiscriminator: "addCssLinksToRoot")]
[JsonDerivedType(typeof(AddProgramServiceAction), typeDiscriminator: "addProgramService")]
public abstract record RequiredAction;

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

public sealed record AddNugetDependencyAction(string PackageName, string Version) : RequiredAction;

public sealed record CopyJsAction(string JsFileName) : RequiredAction;

public sealed record CopyCssAction(string CssFileName, bool Link = false) : RequiredAction;

public sealed record LinkExternalJsAction(string PackageName, string Version, bool Link = false) : RequiredAction;

public sealed record MergeToImportsAction(string[] Namespaces) : RequiredAction;

public sealed record AddCssLinksToRootAction : RequiredAction;

public sealed record AddProgramServiceAction(string UsingNamespace, string ServiceCall) : RequiredAction;
