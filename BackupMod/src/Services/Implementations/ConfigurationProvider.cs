using System;
using BackupMod.Services.Abstractions;
using Microsoft.Extensions.Configuration;

namespace BackupMod.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly string _filePath;
    private readonly IFileService _fileService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(
        string filePath,
        IFileService fileService,
        IJsonSerializer jsonSerializer,
        ILogger<ConfigurationService> logger)
    {
        _filePath = filePath;
        _fileService = fileService;
        _jsonSerializer = jsonSerializer;
        _logger = logger;
    }

    public Configuration GetConfiguration()
    {
        try
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile(_filePath).Build();
            
            var configuration = configurationRoot.Get<Configuration>();

            if (configuration.General.BackupsLimit <= 0)
            {
                _logger.Error("General.BackupsLimit value must be greater than 0.");
                _logger.Warning("The default value will be used until then.");

                configuration.General.BackupsLimit = Configuration.Default.General.BackupsLimit;
            }
            
            if (configuration.Archive.BackupsLimit <= 0)
            {
                _logger.Error("Archive.BackupsLimit value must be greater than 0.");
                _logger.Warning("The default value will be used until then.");

                configuration.Archive.BackupsLimit = Configuration.Default.Archive.BackupsLimit;
            }

            if (configuration.AutoBackup.Delay < 10)
            {
                _logger.Error("AutoBackup.Delay value must be greater than 10 or equals.");
                _logger.Warning("The default value will be used until then.");
                
                configuration.AutoBackup.Delay = Configuration.Default.AutoBackup.Delay;
            }

            if (configuration.General.CustomBackupsFolder == null)
            {
                _logger.Error("General.CustomBackupsFolder value must exist.");
                _logger.Warning("The default value will be used until then.");
                
                configuration.General.CustomBackupsFolder = Configuration.Default.General.CustomBackupsFolder;
            }
            
            if (configuration.Archive.CustomArchiveFolder == null)
            {
                _logger.Error("Archive.CustomArchiveFolder value must exist.");
                _logger.Warning("The default value will be used until then.");
                
                configuration.Archive.CustomArchiveFolder = Configuration.Default.Archive.CustomArchiveFolder;
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

    public bool TryUpdateConfiguration(Configuration configuration)
    {
        try
        {
            string json = _jsonSerializer.Serialize(configuration);
            _fileService.WriteAllText(_filePath, json);

            return true;
        }
        catch (Exception exception)
        {
            _logger.Error("Cannot update the configuration.");
            _logger.Exception(exception);
            
            return false;
        }
    }
}