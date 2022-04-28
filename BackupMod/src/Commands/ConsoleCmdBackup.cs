using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using BackupMod.DI;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Commands;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ConsoleCmdBackup : ConsoleCmdAbstract
{
    private readonly IWorldService _worldService = ServiceLocator.GetRequiredService<IWorldService>();
    private readonly IChatService _chatService = ServiceLocator.GetService<IChatService>();
    private readonly IConfigurationProvider _configurationProvider = ServiceLocator.GetRequiredService<IConfigurationProvider>();
    private readonly ILogger<ConsoleCmdBackup> _logger = ServiceLocator.GetRequiredService<ILogger<ConsoleCmdBackup>>();

    public override bool IsExecuteOnClient => false;

    public override bool AllowedInMainMenu => true;

    public override string[] GetCommands() => new[]
    {
        "backup",
        "bp"
    };

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public override async void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        switch (_params.Count)
        {
            case 0:
                await BackupInternal();

                return;
            case 1:
            {
                if (_params[0] == "info")
                {
                    BackupInfoInternal();
                }
                else
                {
                    _logger.Error("Unknown command. Check that the command is entered correctly.");
                }

                return;
            }
            default:
                _logger.Error("Unknown command. Wrong amount of arguments.");
                break;
        }
    }

    public override string GetDescription() => "make a backup of the current save (command from the BackupMod)";

    private void BackupInfoInternal()
    {
        Configuration configuration = _configurationProvider.GetConfiguration();

        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(); 
        
        _logger.Debug($"BackupMod v{assemblyVersion.Substring(0, assemblyVersion.Length - 2)} by ntaklive");
        _logger.Debug("Current settings:");
        _logger.Debug($"AutoBackupDelay: {configuration.AutoBackupDelay.ToString()}");
        _logger.Debug($"BackupsLimit: {configuration.BackupsLimit.ToString()}");
        _logger.Debug($"EnableChatMessages: {configuration.EnableChatMessages.ToString()}");
        _logger.Debug($"BackupOnWorldLoaded: {configuration.BackupOnWorldLoaded.ToString()}");
        _logger.Debug($"CustomBackupsFolder: {configuration.CustomBackupsFolder}");
    }

    private async Task BackupInternal()
    {
       
        if (_worldService.GetCurrentWorld() != null)
        {
            var backupService = ServiceLocator.GetRequiredService<IWorldBackupService>();
            
            SaveInfo saveInfo = _worldService.GetCurrentWorldSaveInfo();

            string backupFilePath = await backupService.BackupAsync(saveInfo, BackupMode.SaveAllAndBackup);

            _logger.Debug("The manual backup was successfully completed.");
            _logger.Debug($"The backup file location: \"{backupFilePath}\".");
            
            _chatService?.SendMessage("The manual backup was successfully completed.");
        }
        else
        {
            _logger.Error("This command can only be executed when a game is started.");
        }
    }
}