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

/// <summary>
/// Action to add one or more services to the dependency injection container.
/// </summary>
public sealed record AddToServicesAction : RequiredAction
{
    /// <summary>
    /// Service lifetime for registration.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>Transient lifetime.</summary>
        Transient,
        /// <summary>Scoped lifetime (default for Blazor).</summary>
        Scoped,
        /// <summary>Singleton lifetime.</summary>
        Singleton
    }

    /// <summary>Gets the service lifetime.</summary>
    public ServiceLifetime Lifetime { get; }
    /// <summary>Gets the interface type name, if any.</summary>
    public string? InterfaceType { get; }
    /// <summary>Gets the implementation type name.</summary>
    public string ImplementationType { get; }

    /// <summary>
    /// Initializes an AddToServicesAction with lifetime, interface, and implementation.
    /// </summary>
    public AddToServicesAction(ServiceLifetime lifetime, string interfaceType, string implementationType)
    {
        Lifetime = lifetime;
        InterfaceType = interfaceType;
        ImplementationType = implementationType;
    }

    /// <summary>
    /// Initializes an AddToServicesAction with interface and implementation (defaulting to Scoped).
    /// </summary>
    public AddToServicesAction(string interfaceType, string implementationType)
    {
        Lifetime = ServiceLifetime.Scoped;
        InterfaceType = interfaceType;
        ImplementationType = implementationType;
    }

    /// <summary>
    /// Initializes an AddToServicesAction with implementation (defaulting to Scoped, no interface).
    /// </summary>
    public AddToServicesAction(string implementationType)
    {
        Lifetime = ServiceLifetime.Scoped;
        ImplementationType = implementationType;
    }
}

/// <summary>Action to add a NuGet package dependency.</summary>
/// <param name="PackageName">The name of the package.</param>
/// <param name="Version">The package version.</param>
public sealed record AddNugetDependencyAction(string PackageName, string Version) : RequiredAction;

/// <summary>Action to copy a JavaScript file into the project.</summary>
/// <param name="JsFileName">The name of the JavaScript file to copy.</param>
public sealed record CopyJsAction(string JsFileName) : RequiredAction;

/// <summary>Action to copy a CSS file into the project.</summary>
/// <param name="CssFileName">The name of the CSS file to copy.</param>
/// <param name="Link">Whether to automatically link the CSS in the root HTML.</param>
public sealed record CopyCssAction(string CssFileName, bool Link = false) : RequiredAction;

/// <summary>Action to link an external JavaScript package via CDN.</summary>
/// <param name="PackageName">The name of the package.</param>
/// <param name="Version">The package version.</param>
/// <param name="Link">Whether to automatically link the JS in the root HTML.</param>
public sealed record LinkExternalJsAction(string PackageName, string Version, bool Link = false) : RequiredAction;

/// <summary>Action to merge namespaces into the global imports.</summary>
/// <param name="Namespaces">The namespaces to add.</param>
public sealed record MergeToImportsAction(string[] Namespaces) : RequiredAction;

/// <summary>Action to add standard CSS links to the root document.</summary>
public sealed record AddCssLinksToRootAction : RequiredAction;

/// <summary>Action to add a specific service configuration call to Program.cs.</summary>
/// <param name="UsingNamespace">The namespace to add a 'using' for.</param>
/// <param name="ServiceCall">The service configuration call (e.g., 'AddTailwindMerge()').</param>
public sealed record AddProgramServiceAction(string UsingNamespace, string ServiceCall) : RequiredAction;
