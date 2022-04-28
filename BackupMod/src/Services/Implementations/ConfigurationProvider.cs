using System;
using System.Text.Json;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.Configuration;

namespace BackupMod.Services;

public class ConfigurationProvider : Abstractions.IConfigurationProvider
{
    private readonly string _filePath;
    private readonly ILogger<ConfigurationProvider> _logger;
    private IConfigurationRoot _configurationRoot;


    public ConfigurationProvider(
        string filePath,
        ILogger<ConfigurationProvider> logger)
    {
        _filePath = filePath;
        _logger = logger;

        LoadConfiguration(filePath);
    }

    public Configuration GetConfiguration()
    {
        if (_configurationRoot == null)
        {
            return Configuration.Default;
        }

        LoadConfiguration(_filePath);
        return _configurationRoot.Get<Configuration>();

    }

    private void LoadConfiguration(string filePath)
    {
        try
        {
            _configurationRoot = new ConfigurationBuilder()
                .AddJsonFile(filePath).Build();
        }
        catch
        {
            _logger.Error("JSON format of your 'settings.json' file is incorrect.");
            _logger.Error("Make sure that you escaped all the '\\' characters.");
            _logger.Error("After fixing the configuration, you have to restart the game");
            _logger.Error("The default configuration will be used until then.");
        }
    }
}