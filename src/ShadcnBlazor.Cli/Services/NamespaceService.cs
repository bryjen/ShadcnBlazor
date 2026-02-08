using System.Text.RegularExpressions;

namespace ShadcnBlazor.Cli.Services;

public class NamespaceService
{
    public string ReplaceNamespaceInRazor(string content, string? newNamespace = null)
    {
        // Remove or replace @namespace directive
        var namespacePattern = @"@namespace\s+[\w.]+(?:\r?\n)?";
        var hasNamespace = Regex.IsMatch(content, @"@namespace\s+[\w.]+", RegexOptions.Multiline);
        
        if (newNamespace is null)
        {
            // Remove @namespace directive
            content = Regex.Replace(content, namespacePattern, string.Empty, RegexOptions.Multiline);
        }
        else
        {
            if (hasNamespace)
            {
                // Replace existing @namespace directive
                content = Regex.Replace(content, @"@namespace\s+[\w.]+", $"@namespace {newNamespace}", RegexOptions.Multiline);
            }
            else
            {
                // Add @namespace directive at the beginning of the file
                content = $"@namespace {newNamespace}\n{content}";
            }
        }
        
        return content;
    }
    
    public string ReplaceNamespaceInCs(string content, string? newNamespace = null)
    {
        // Handle file-scoped namespace: namespace X;
        var fileScopedPattern = @"namespace\s+([\w.]+)\s*;";
        
        // Handle block-scoped namespace: namespace X { ... }
        var blockScopedPattern = @"namespace\s+([\w.]+)\s*\{";
        
        var hasFileScoped = Regex.IsMatch(content, fileScopedPattern, RegexOptions.Multiline);
        var hasBlockScoped = Regex.IsMatch(content, blockScopedPattern, RegexOptions.Multiline);
        var hasNamespace = hasFileScoped || hasBlockScoped;
        
        if (newNamespace is null)
        {
            // Remove file-scoped namespace
            content = Regex.Replace(content, fileScopedPattern + @"\s*", string.Empty, RegexOptions.Multiline);
            
            // Remove block-scoped namespace and unindent content
            content = Regex.Replace(content, blockScopedPattern, string.Empty, RegexOptions.Multiline);
            
            // Remove closing brace if it was a namespace block
            // This is a simple approach - matches closing brace at start of line with proper indentation
            content = Regex.Replace(content, @"^\s*}\s*$", string.Empty, RegexOptions.Multiline);
            
            // Unindent all lines by 4 spaces (assuming standard indentation)
            var lines = content.Split('\n');
            var unindented = lines.Select(line =>
            {
                if (string.IsNullOrWhiteSpace(line))
                    return line;
                
                // Remove 4 spaces of indentation if present
                if (line.StartsWith("    "))
                    return line.Substring(4);
                
                return line;
            });
            
            content = string.Join("\n", unindented);
        }
        else
        {
            if (hasFileScoped)
            {
                // Replace file-scoped namespace
                content = Regex.Replace(content, fileScopedPattern, $"namespace {newNamespace};", RegexOptions.Multiline);
            }
            else if (hasBlockScoped)
            {
                // Replace block-scoped namespace
                content = Regex.Replace(content, blockScopedPattern, $"namespace {newNamespace} {{", RegexOptions.Multiline);
            }
            else
            {
                // Add file-scoped namespace at the beginning (after using statements)
                var usingStatements = Regex.Matches(content, @"^using\s+[^;]+;", RegexOptions.Multiline);
                var lastUsingIndex = usingStatements.Count > 0 
                    ? usingStatements[^1].Index + usingStatements[^1].Length 
                    : 0;
                
                var namespaceLine = $"\nnamespace {newNamespace};\n";
                content = content.Insert(lastUsingIndex, namespaceLine);
            }
        }
        
        return content;
    }
    
    public void ProcessFile(FileInfo file, string? newNamespace = null)
    {
        var content = File.ReadAllText(file.FullName);
        var processed = file.Extension == ".razor"
            ? ReplaceNamespaceInRazor(content, newNamespace)
            : ReplaceNamespaceInCs(content, newNamespace);
        
        File.WriteAllText(file.FullName, processed);
    }
}
