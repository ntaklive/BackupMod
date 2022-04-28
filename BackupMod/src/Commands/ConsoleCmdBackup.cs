using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BackupMod.DI;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Commands;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ConsoleCmdBackup : ConsoleCmdAbstract
{
    private readonly IWorldBackupService _backupService = ServiceLocator.GetRequiredService<IWorldBackupService>();
    private readonly IWorldService _worldService = ServiceLocator.GetRequiredService<IWorldService>();
    private readonly IChatService _chatService = ServiceLocator.GetService<IChatService>();
    private readonly IConfigurationProvider _configurationProvider = ServiceLocator.GetRequiredService<IConfigurationProvider>();
    private readonly ILogger<ConsoleCmdBackup> _logger = ServiceLocator.GetRequiredService<ILogger<ConsoleCmdBackup>>();

    public override bool IsExecuteOnClient => false;

    public override bool AllowedInMainMenu => true;

    public override string[] GetCommands() => new string[2]
    {
        "backup",
        "bp"
    };

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        switch (_params.Count)
        {
            case 0:
                BackupInternal();

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

        _logger.Debug($"AutoBackupDelay: {configuration.AutoBackupDelay.ToString()}");
        _logger.Debug($"BackupsLimit: {configuration.BackupsLimit.ToString()}");
        _logger.Debug($"EnableChatMessages: {configuration.EnableChatMessages.ToString()}");
        _logger.Debug($"BackupOnWorldLoaded: {configuration.BackupOnWorldLoaded.ToString()}");
        _logger.Debug($"CustomBackupsFolder: {configuration.CustomBackupsFolder}");
    }

    private void BackupInternal()
    {
        if (_worldService.GetCurrentWorld() != null)
        {
            SaveInfo saveInfo = _worldService.GetCurrentWorldSaveInfo();

            _backupService.Backup(saveInfo, BackupMode.SaveAllAndBackup);

            _logger.Debug("The manual backup was successfully completed.");
            _chatService?.SendMessage("The manual backup was successfully completed.");
        }
        else
        {
            _logger.Error("This command can only be executed when a game is started.");
        }
    }
}