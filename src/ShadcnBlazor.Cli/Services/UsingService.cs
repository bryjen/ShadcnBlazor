using System.Text.RegularExpressions;

namespace ShadcnBlazor.Cli.Services;

public class UsingService
{
    public string ReplaceUsingsInRazor(string content, string oldNamespacePrefix, string newNamespacePrefix)
    {
        // Replace @using directives: @using OldNamespace.X -> @using NewNamespace.X
        var usingPattern = $@"@using\s+{Regex.Escape(oldNamespacePrefix)}\.";
        content = Regex.Replace(content, usingPattern, $"@using {newNamespacePrefix}.", RegexOptions.Multiline);
        
        return content;
    }
    
    public string ReplaceUsingsInCs(string content, string oldNamespacePrefix, string newNamespacePrefix)
    {
        // Replace using statements: using OldNamespace.X; -> using NewNamespace.X;
        var usingPattern = $@"using\s+{Regex.Escape(oldNamespacePrefix)}\.";
        content = Regex.Replace(content, usingPattern, $"using {newNamespacePrefix}.", RegexOptions.Multiline);
        
        return content;
    }
}
