using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ShadcnBlazor.Cli.Services;

public class ConfigService
{
    private const string ConfigFileName = "shadcn-blazor.yaml";
    
    public OutputProjectConfig LoadConfig(DirectoryInfo workingDirectory)
    {
        var configFileInfo = workingDirectory.EnumerateFiles()
            .FirstOrDefault(f => f.Name == ConfigFileName);
        
        if (configFileInfo is null)
        {
            throw new ConfigFileNotFoundException(ConfigFileName);
        }
        
        var configFileContents = File.ReadAllText(configFileInfo.FullName);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        
        return deserializer.Deserialize<OutputProjectConfig>(configFileContents);
    }
    
    public OutputProjectConfig SaveConfig(DirectoryInfo workingDirectory, OutputProjectConfig config)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(config);
        
        var fileInfo = new FileInfo(Path.Join(workingDirectory.FullName, ConfigFileName));
        File.WriteAllText(fileInfo.FullName, yaml);
        
        return config;
    }
}
