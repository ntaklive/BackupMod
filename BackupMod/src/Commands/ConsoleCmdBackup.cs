using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using BackupMod.DI;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;
using UniLinq;

namespace BackupMod.Commands;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ConsoleCmdBackup : ConsoleCmdAbstract
{
    private readonly IWorldService _worldService = ServiceLocator.GetRequiredService<IWorldService>();
    private readonly ISavesProvider _savesProvider = ServiceLocator.GetRequiredService<ISavesProvider>();
    private readonly IChatService _chatService = ServiceLocator.GetService<IChatService>();
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
                else if (_params[0] == "restore")
                {
                    BackupRestoreInternal(null, null, null);
                }
                else
                {
                    LogUnknownCommand();
                }

                return;
            }
            case 4:
            {
                if (_params[0] == "restore")
                {
                    if (!int.TryParse(_params[1], out int worldId))
                    {
                        LogInvalidArgument("world id", _params[1]);

                        return;
                    }

                    if (!int.TryParse(_params[2], out int saveId))
                    {
                        LogInvalidArgument("save id", _params[2]);

                        return;
                    }

                    if (!int.TryParse(_params[3], out int backupId))
                    {
                        LogInvalidArgument("backup id", _params[3]);

                        return;
                    }

                    BackupRestoreInternal(worldId, saveId, backupId);
                }
                else
                {
                    LogUnknownCommand();
                }

                return;
            }
            default:
                LogWrongArgumentsAmount();

                return;
        }
    }

    public override string GetDescription() => "some commands to simplify the creation of backups (command from the BackupMod)";

    private void BackupRestoreInternal(int? worldId, int? saveId, int? backupId)
    {
        if (_worldService.GetCurrentWorld() != null)
        {
            _logger.Error("This command can only be executed in the main menu.");

            return;
        }

        WorldInfo[] worlds = _savesProvider.GetAllWorlds().ToArray();

        if (worldId == null || saveId == null || backupId == null)
        {
            _logger.Debug("Please specify a backup to restore. Hint: '/backup restore *worldId* *saveId* *backupId*'.");

            PrintAvailableBackups(worlds);

            return;
        }

        int worldsAmount = worlds.Length;
        if (worldId >= worldsAmount || worldId < 0)
        {
            if (worldsAmount != 0)
            {
                _logger.Error($"Invalid world id. Please specify a world id in the range 0-{worldsAmount - 1}.");
            }
            else
            {
                LogThereIsNoBackups();
            }
            
            return;
        }

        int savesAmount = worlds[worldId.Value].Saves.Length;
        if (saveId >= savesAmount || saveId < 0)
        {
            if (savesAmount != 0)
            {
                _logger.Error($"Invalid save id. Please specify a save id in the range 0-{savesAmount - 1}.");
            }
            else
            {
                LogThereIsNoBackups();
            }
            
            return;
        }

        int backupAmount = worlds[worldId.Value].Saves[saveId.Value].Backups.Length;
        if (backupId >= backupAmount || backupId < 0)
        {
            if (backupAmount != 0)
            {
                _logger.Error($"Invalid backup id. Please specify a backup id in the range 0-{backupAmount - 1}.");
            }
            else
            {
                LogThereIsNoBackups();
            }
            
            return;
        }

        var backupService = ServiceLocator.GetRequiredService<IWorldBackupService>();

        try
        {
            backupService.RestoreAsync(worlds[worldId.Value].Saves[saveId.Value].Backups[backupId.Value]);
        }
        catch (Exception exception)
        {
            _logger.Exception(exception);
            
            return;
        }
        
        _logger.Debug("The selected save's backup was successfully restored.");
    }
    
    private void PrintAvailableBackups(IReadOnlyList<WorldInfo> worlds)
    {
        int worldsAmount = worlds.Count;

        _logger.Debug("Available backups:");

        if (worldsAmount == 0)
        {
            _logger.Warning("There is no worlds.");
            _logger.Warning("First, you should make a backup of any save.");
        }

        for (var i = 0; i < worldsAmount; i++)
        {
            WorldInfo world = worlds[i];

            _logger.Debug($"[{i}]: {world.WorldName}");

            int savesAmount = world.Saves.Length;
            if (savesAmount == 0)
            {
                _logger.Debug("    There is no saves.");
            }
            
            for (var j = 0; j < savesAmount; j++)
            {
                SaveInfo save = world.Saves[j];

                _logger.Debug($"    [{j}]: {save.SaveName}");

                int backupsAmount = save.Backups.Length;
                if (backupsAmount == 0)
                {
                    _logger.Debug("        There is no backups.");
                }

                for (var k = 0; k < backupsAmount; k++)
                {
                    BackupInfo backup = save.Backups[k];

                    _logger.Debug($"        [{k}]: {backup.Timestamp:MM.dd.yyyy HH:mm:ss}");
                }
            }
        }
    }

    private void BackupInfoInternal()
    {
        var configuration = ServiceLocator.GetRequiredService<Configuration>();

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
        if (_worldService.GetCurrentWorld() == null)
        {
            _logger.Error("This command can only be executed when a game is started.");

            return;
        }

        var backupService = ServiceLocator.GetRequiredService<IWorldBackupService>();

        SaveInfo saveInfo = _worldService.GetCurrentWorldSaveInfo();

        string backupFilePath = await backupService.BackupAsync(saveInfo, BackupMode.SaveAllAndBackup);

        _logger.Debug("The manual backup was successfully completed.");
        _logger.Debug($"The backup file location: \"{backupFilePath}\".");

        _chatService?.SendMessage("The manual backup was successfully completed.");
    }

    private void LogInvalidArgument(string argumentName, string argumentValue) => _logger.Error($"'{argumentValue}' is an invalid {argumentName}.");

    private void LogUnknownCommand() => _logger.Error("Unknown command. Check that the command is entered correctly.");

    private void LogWrongArgumentsAmount() => _logger.Error("Unknown command. Wrong amount of arguments.");
    
    private void LogThereIsNoBackups() => _logger.Error("There is no backups. First you should make a backup of any save");
}