using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.Configuration;

namespace BackupMod.Services;

public class ConfigurationProvider : Abstractions.IConfigurationProvider
{
    private readonly IConfigurationRoot _configurationRoot;

    public ConfigurationProvider(
        string filePath
    )
    {
        _configurationRoot = new ConfigurationBuilder()
            .AddJsonFile(filePath).Build();
    }

    public Configuration GetConfiguration()
    {
        _configurationRoot.Reload();
        return _configurationRoot.Get<Configuration>();
    }
}