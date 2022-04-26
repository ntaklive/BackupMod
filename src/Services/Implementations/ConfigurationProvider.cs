using Microsoft.Extensions.Configuration;
using NtakliveBackupMod.Scripts.Services.Abstractions.Models;
using IConfigurationProvider = NtakliveBackupMod.Scripts.Services.Abstractions.IConfigurationProvider;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class ConfigurationProvider : IConfigurationProvider
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