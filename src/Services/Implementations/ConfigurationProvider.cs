using Microsoft.Extensions.Configuration;
using NtakliveBackupMod.Services.Abstractions.Models;

namespace NtakliveBackupMod.Services.Implementations;

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