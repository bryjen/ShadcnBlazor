namespace ShadcnBlazor.Internal.Utilities;

public static class ComponentPathHelper
{
    public static string GetJsModulePath<TComponent>(string? fileName = null)
    {
        return GetJsModulePath(typeof(TComponent), fileName);
    }

    public static string GetJsModulePath(Type type, string? fileName = null)
    {
        var assemblyName = type.Assembly.GetName().Name;
        var fullName = type.FullName ?? throw new InvalidOperationException("Type FullName is null");
        
        string relativePath;
        if (fileName != null)
        {
            // Use the namespace of the type but replace the type name with the fileName
            var namespaceName = type.Namespace ?? "";
            if (namespaceName.StartsWith(assemblyName + "."))
            {
                namespaceName = namespaceName[(assemblyName.Length + 1)..];
            }
            relativePath = namespaceName.Replace(".", "/") + "/" + fileName;
        }
        else
        {
            relativePath = fullName;
            if (fullName.StartsWith(assemblyName + "."))
            {
                relativePath = fullName[(assemblyName.Length + 1)..];
            }
            relativePath = relativePath.Replace(".", "/") + ".razor.js";
        }
            
        return $"/_content/{assemblyName}/{relativePath.TrimStart('/')}";
    }

    public static string GetWwwrootJsPath<T>(string fileName)
    {
        var assemblyName = typeof(T).Assembly.GetName().Name;
        return $"/_content/{assemblyName}/js/{fileName}";
    }
}
