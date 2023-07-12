using System;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly string _filePath;
    private readonly IFileService _fileService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(
        string filepath,
        IFileService fileService,
        IJsonSerializer jsonSerializer,
        ILogger<ConfigurationService> logger)
    {
        _filePath = filepath;
        _fileService = fileService;
        _jsonSerializer = jsonSerializer;
        _logger = logger;
    }

    public ModConfiguration ReadConfiguration()
    {
        try
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile(_filePath).Build();
            
            var configuration = configurationRoot.Get<ModConfiguration>();

            if (configuration.General.BackupsLimit <= 0)
            {
                _logger.LogError("General.BackupsLimit value must be greater than 0");
                _logger.LogWarning("The default value will be used until then");

                configuration.General.BackupsLimit = ModConfiguration.Default.General.BackupsLimit;
            }
            
            if (configuration.Archive.BackupsLimit <= 0)
            {
                _logger.LogError("Archive.BackupsLimit value must be greater than 0");
                _logger.LogWarning("The default value will be used until then");

                configuration.Archive.BackupsLimit = ModConfiguration.Default.Archive.BackupsLimit;
            }

            if (configuration.AutoBackup.Delay < 10)
            {
                _logger.LogError("AutoBackup.Delay value must be greater than 10 or equals");
                _logger.LogWarning("The default value will be used until then");
                
                configuration.AutoBackup.Delay = ModConfiguration.Default.AutoBackup.Delay;
            }            
            
            if (configuration.Notifications.Countdown.CountFrom < 1)
            {
                _logger.LogError("Notifications.Countdown.CountFrom value must be greater than 1 or equals");
                _logger.LogWarning("The default value will be used until then");
                
                configuration.Notifications.Countdown.CountFrom = ModConfiguration.Default.Notifications.Countdown.CountFrom;
            }

            if (configuration.General.CustomBackupsFolder == null)
            {
                _logger.LogError("General.CustomBackupsFolder value must exist");
                _logger.LogWarning("The default value will be used until then");
                
                configuration.General.CustomBackupsFolder = ModConfiguration.Default.General.CustomBackupsFolder;
            }
            
            if (configuration.Archive.CustomArchiveFolder == null)
            {
                _logger.LogError("Archive.CustomArchiveFolder value must exist");
                _logger.LogWarning("The default value will be used until then");
                
                configuration.Archive.CustomArchiveFolder = ModConfiguration.Default.Archive.CustomArchiveFolder;
            }
            
            if (configuration.General.CustomBackupsFolder == configuration.Archive.CustomArchiveFolder &&
                !string.IsNullOrWhiteSpace(configuration.General.CustomBackupsFolder) && string.IsNullOrWhiteSpace(configuration.Archive.CustomArchiveFolder))
            {
                _logger.LogError("General.CustomBackupsFolder must be not equal to Archive.CustomArchiveFolder");
                _logger.LogWarning("The default value for each property will be used");
                
                configuration.General.CustomBackupsFolder = ModConfiguration.Default.General.CustomBackupsFolder;
                configuration.Archive.CustomArchiveFolder = ModConfiguration.Default.Archive.CustomArchiveFolder;
            }

            return configuration;
        }
        catch (Exception exception)
        {
            _logger.LogError("JSON format of your 'settings.json' file is incorrect");
            _logger.LogError("Make sure that you escaped all the '\\' characters");
            _logger.LogError("You should fix the configuration file");
            _logger.LogError("The default configuration will be used until then");
            
            _logger.LogWarning(exception.ToString());
            
            return ModConfiguration.Default;
        }
    }

    public bool TryUpdateConfiguration(ModConfiguration configuration)
    {
        try
        {
            string json = _jsonSerializer.Serialize(configuration);
            _fileService.WriteAllText(_filePath, json);

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError("Cannot update the configuration");
            _logger.LogCritical(exception, "A critical error was occured");
            
            return false;
        }
    }
}