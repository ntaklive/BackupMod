using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.Configuration;

namespace BackupMod.Services;

public class ConfigurationProvider : Abstractions.IConfigurationProvider
{
    private readonly string _filePath;
    private readonly ILogger<ConfigurationProvider> _logger;

    public ConfigurationProvider(
        string filePath,
        ILogger<ConfigurationProvider> logger)
    {
        _filePath = filePath;
        _logger = logger;
    }

    public Configuration GetConfiguration()
    {
        try
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .AddJsonFile(_filePath).Build();
            
            var configuration = configurationRoot.Get<Configuration>();

            if (configuration.BackupsLimit <= 0)
            {
                _logger.Error("BackupsLimit value must be greater than 0.");
                _logger.Warning("The default value will be used until then.");

                configuration.BackupsLimit = Configuration.Default.BackupsLimit;
            }

            if (configuration.AutoBackupDelay < 10)
            {
                _logger.Error("AutoBackupDelay value must be greater than 10 or equals.");
                _logger.Warning("The default value will be used until then.");
                
                configuration.AutoBackupDelay = Configuration.Default.AutoBackupDelay;
            }

            if (configuration.CustomBackupsFolder == null)
            {
                _logger.Error("CustomBackupsFolder value must exist.");
                _logger.Warning("The default value will be used until then.");
                
                configuration.CustomBackupsFolder = Configuration.Default.CustomBackupsFolder;
            }

            return configuration;
        }
        catch
        {
            _logger.Error("JSON format of your 'settings.json' file is incorrect.");
            _logger.Error("Make sure that you escaped all the '\\' characters.");
            _logger.Error("You should fix the configuration file.");
            _logger.Error("The default configuration will be used until then.");
            
            return Configuration.Default;
        }
    }
}