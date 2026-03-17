namespace ShadcnBlazor.Cli.Services;

public class FileSystemService
{
    private static readonly string[] TemplateExcludeDirs = ["obj", "bin"];

    public void CopyDirectory(string sourceDir, string destDir)
    {
        CopyDirectory(sourceDir, destDir, excludeDirs: null);
    }

    public void CopyTemplateDirectory(string sourceDir, string destDir)
    {
        CopyDirectory(sourceDir, destDir, TemplateExcludeDirs);
    }

    private static void CopyDirectory(string sourceDir, string destDir, string[]? excludeDirs)
    {
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(destDir, fileName), overwrite: true);
        }

        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(subDir);
            if (excludeDirs is not null && excludeDirs.Contains(dirName, StringComparer.OrdinalIgnoreCase))
                continue;
            CopyDirectory(subDir, Path.Combine(destDir, dirName), excludeDirs);
        }
    }
}
