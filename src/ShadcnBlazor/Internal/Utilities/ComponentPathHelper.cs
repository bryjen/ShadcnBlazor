namespace ShadcnBlazor.Internal.Utilities;

/// <summary>
/// Helper for generating resource paths for components.
/// </summary>
public static class ComponentPathHelper
{
    /// <summary>
    /// Gets the JavaScript module path for a specific component type.
    /// </summary>
    /// <typeparam name="TComponent">The component type.</typeparam>
    /// <param name="fileName">Optional filename override.</param>
    /// <returns>The content path to the JavaScript module.</returns>
    public static string GetJsModulePath<TComponent>(string? fileName = null)
    {
        return GetJsModulePath(typeof(TComponent), fileName);
    }

    /// <summary>
    /// Gets the JavaScript module path for a specific type.
    /// </summary>
    /// <param name="type">The type to get the path for.</param>
    /// <param name="fileName">Optional filename override.</param>
    /// <returns>The content path to the JavaScript module.</returns>
    public static string GetJsModulePath(Type type, string? fileName = null)
    {
        var assemblyName = type.Assembly.GetName().Name ?? string.Empty;
        var fullName = type.FullName ?? throw new InvalidOperationException("Type FullName is null");
        
        string relativePath;
        if (fileName != null)
        {
            // Use the namespace of the type but replace the type name with the fileName
            var namespaceName = type.Namespace ?? "";
            if (!string.IsNullOrEmpty(assemblyName) && namespaceName.StartsWith(assemblyName + "."))
            {
                namespaceName = namespaceName[(assemblyName.Length + 1)..];
            }
            relativePath = namespaceName.Replace(".", "/") + "/" + fileName;
        }
        else
        {
            relativePath = fullName;
            if (!string.IsNullOrEmpty(assemblyName) && fullName.StartsWith(assemblyName + "."))
            {
                relativePath = fullName[(assemblyName.Length + 1)..];
            }
            relativePath = relativePath.Replace(".", "/") + ".razor.js";
        }
            
        return $"/_content/{assemblyName}/{relativePath.TrimStart('/')}";
    }

    /// <summary>
    /// Gets the path to a JavaScript file in the wwwroot/js directory of the assembly containing <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type used to identify the assembly.</typeparam>
    /// <param name="fileName">The JavaScript filename.</param>
    /// <returns>The content path to the JavaScript file.</returns>
    public static string GetWwwrootJsPath<T>(string fileName)
    {
        var assemblyName = typeof(T).Assembly.GetName().Name ?? string.Empty;
        return $"/_content/{assemblyName}/js/{fileName}";
    }
}
