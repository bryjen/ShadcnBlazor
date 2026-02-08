namespace ShadcnBlazor.Cli.Services;

public class FileSystemService
{
    public void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);
    
        // Copy files
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(destDir, fileName), overwrite: true);
        }
    
        // Copy subdirectories recursively
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(subDir);
            CopyDirectory(subDir, Path.Combine(destDir, dirName));
        }
    }
}
