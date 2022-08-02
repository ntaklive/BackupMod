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
    private readonly IGameDataProvider _gameDataProvider = ServiceLocator.GetRequiredService<IGameDataProvider>();
    private readonly IGameDirectoriesService _gameDirectoriesService = ServiceLocator.GetRequiredService<IGameDirectoriesService>();
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
                switch (_params[0])
                {
                    case "info":
                        BackupInfoInternal();
                        break;
                    case "restore":
                        await BackupRestoreInternal(null, null, null);
                        break;
                    case "delete":
                        await BackupDeleteInternal(null, null, null);
                        break;
                    case "list":
                        BackupListInternal();
                        break;
                    default:
                        LogUnknownCommand();
                        break;
                }

                return;
            }
            case 4:
            {
                switch (_params[0])
                {
                    case "restore":
                    {
                        if (!TryParseArguments(
                                _params[1], _params[2], _params[3],
                                out int worldId, out int saveId, out int backupId)
                           )
                        {
                            return;
                        }

                        await BackupRestoreInternal(worldId, saveId, backupId);
                        break;
                    }
                    case "delete":
                    {
                        if (!TryParseArguments(
                                _params[1], _params[2], _params[3],
                                out int worldId, out int saveId, out int backupId)
                           )
                        {
                            return;
                        }

                        await BackupDeleteInternal(worldId, saveId, backupId);
                        break;
                    }
                    default:
                        LogUnknownCommand();
                        break;
                }

                return;
            }
            default:
                LogWrongArgumentsAmount();

                return;
        }
    }


    public override string GetDescription() => "some commands to simplify the creation of backups (command from the BackupMod)";

    private bool TryParseArguments(string worldIdParam, string saveIdParam, string backupIdParam, out int worldId, out int  saveId, out int  backupId)
    {
        var isValid = true;
        
        if (!int.TryParse(worldIdParam, out worldId))
        {
            LogInvalidArgument("world id", worldIdParam);

            isValid = false;
        }

        if (!int.TryParse(saveIdParam, out saveId))
        {
            LogInvalidArgument("save id", saveIdParam);

            isValid = false;
        }

        if (!int.TryParse(backupIdParam, out backupId))
        {
            LogInvalidArgument("backup id", backupIdParam);

            isValid = false;
        }

        return isValid;
    }
    
    private void BackupListInternal()
    {
        WorldInfo[] worlds = _gameDataProvider.GetWorldsData().ToArray();
        
        PrintAvailableBackups(worlds);
    }

    private async Task BackupRestoreInternal(int? worldId, int? saveId, int? backupId)
    {
        if (_worldService.GetCurrentWorld() != null)
        {
            _logger.Error("This command can only be executed in the main menu.");

            return;
        }

        WorldInfo[] worlds = _gameDataProvider.GetWorldsData().ToArray();

        if (worldId == null || saveId == null || backupId == null)
        {
            _logger.Debug("Please specify a backup to restore. Hint: 'backup restore *worldId* *saveId* *backupId*'.");

            PrintAvailableBackups(worlds);

            return;
        }

        if (!TryValidateArguments(worlds, worldId, saveId, backupId))
        {
            return;
        }

        var backupService = ServiceLocator.GetRequiredService<IWorldBackupService>();

        try
        {
            BackupInfo selectedBackup = worlds[worldId.Value].Saves[saveId.Value].Backups[backupId.Value];
            
            await backupService.RestoreAsync(selectedBackup);
        }
        catch (Exception exception)
        {
            _logger.Exception(exception);
            
            return;
        }
        
        _logger.Debug("The selected save's backup was successfully restored.");
    }
    
    private async Task BackupDeleteInternal(int? worldId, int? saveId, int? backupId)
    {
        if (_worldService.GetCurrentWorld() != null)
        {
            _logger.Error("This command can only be executed in the main menu.");

            return;
        }

        WorldInfo[] worlds = _gameDataProvider.GetWorldsData().ToArray();

        if (worldId == null || saveId == null || backupId == null)
        {
            _logger.Debug("Please specify a backup to delete. Hint: 'backup delete *worldId* *saveId* *backupId*'.");

            PrintAvailableBackups(worlds);

            return;
        }

        if (!TryValidateArguments(worlds, worldId, saveId, backupId))
        {
            return;
        }

        var backupService = ServiceLocator.GetRequiredService<IWorldBackupService>();

        try
        {
            await backupService.DeleteAsync(worlds[worldId.Value].Saves[saveId.Value].Backups[backupId.Value]);
        }
        catch (Exception exception)
        {
            _logger.Exception(exception);
            
            return;
        }
        
        _logger.Debug("The selected save's backup was successfully deleted.");
    }

    private void BackupInfoInternal()
    {
        var configuration = ServiceLocator.GetRequiredService<Configuration>();

        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        _logger.Debug($"BackupMod v{assemblyVersion.Substring(0, assemblyVersion.Length - 2)} by ntaklive");
        _logger.Debug("Current settings:");
        _logger.Debug($"General.BackupsLimit: {configuration.General.BackupsLimit.ToString()}");
        _logger.Debug($"General.CustomBackupsFolder: {configuration.General.CustomBackupsFolder}");
        _logger.Debug($"AutoBackup.Enabled: {configuration.AutoBackup.Enabled.ToString()}");
        _logger.Debug($"AutoBackup.Delay: {configuration.AutoBackup.Delay.ToString()}");
        _logger.Debug($"AutoBackup.SkipIfThereIsNoPlayers: {configuration.AutoBackup.SkipIfThereIsNoPlayers.ToString()}");
        _logger.Debug($"Archive.Enabled: {configuration.Archive.Enabled.ToString()}");
        _logger.Debug($"Archive.BackupsLimit: {configuration.Archive.BackupsLimit.ToString()}");
        _logger.Debug($"Archive.CustomArchiveFolder: {configuration.Archive.CustomArchiveFolder}");
        _logger.Debug($"Events.BackupOnWorldLoaded: {configuration.Events.BackupOnWorldLoaded.ToString()}");
        _logger.Debug($"Events.BackupOnServerIsEmpty: {configuration.Events.BackupOnServerIsEmpty.ToString()}");
        _logger.Debug($"Utilities.ChatNotificationsEnabled: {configuration.Utilities.ChatNotificationsEnabled.ToString()}");
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

                    _logger.Debug($"        [{k}]: {backup.Timestamp:MM.dd.yyyy HH:mm:ss}{(backup.Filepath.Contains(_gameDirectoriesService.GetArchiveFolderPath()) ? " (archived)" : string.Empty) }");
                }
            }
        }
    }

    [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
    private bool TryValidateArguments(IReadOnlyList<WorldInfo> worlds, int? worldId, int? saveId, int? backupId)
    {
        var isValid = true;
        
        int worldsAmount = worlds.Count;
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

            isValid = false;
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
            
            isValid = false;
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
            
            isValid = false;
        }

        return isValid;
    }

    private void LogInvalidArgument(string argumentName, string argumentValue) => _logger.Error($"'{argumentValue}' is an invalid {argumentName}.");

    private void LogUnknownCommand() => _logger.Error("Unknown command. Check that the command is entered correctly.");

    private void LogWrongArgumentsAmount() => _logger.Error("Unknown command. Wrong amount of arguments.");
    
    private void LogThereIsNoBackups() => _logger.Error("There is no backups. First you should make a backup of any save");
}