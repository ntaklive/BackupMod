using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BackupMod.Modules.Commands.Enums;
using BackupMod.Modules.Commands.EventArgs;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Game;
using BackupMod.Services.Abstractions.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Theraot.Collections;

namespace BackupMod.Modules.Commands;

public partial class ConsoleCmdBackup : ConsoleCmdBase
{
    private readonly IWorldService _worldService;
    private readonly IBackupManager _backupManager;
    private readonly IWorldInfoService _worldInfoService;
    [CanBeNull] private readonly IChatService _chatService;
    private readonly ILogger<ConsoleCmdBackup> _logger;
    private readonly string _helpText;

    public ConsoleCmdBackup()
    {
        _worldService = Provider.GetRequiredService<IWorldService>();
        _backupManager = Provider.GetRequiredService<IBackupManager>();
        _worldInfoService = Provider.GetRequiredService<IWorldInfoService>();
        _chatService = Provider.GetService<IChatService>();
        _logger = Provider.GetRequiredService<ILogger<ConsoleCmdBackup>>();

        Dictionary<string, string> commandDescriptionDictionary = new()
        {
            { "", "perform a forceful backup" },
            { "info", "show the current configuration of the mod" },
            { "list", "show all available backups" },
            { "restore", "restore a save from a backup" },
            { "delete", "delete a backup" },
            { "start", "start an AutoBackup process (even if disabled in settings.json)" },
            { "stop", "stop the current AutoBackup process" },
        };
        
        var i = 1;
        var j = 1;
        _helpText = $"Usage:\n  {string.Join("\n  ", commandDescriptionDictionary.Keys.Select(command => $"{i++}. {GetCommands()[0]} {command}").ToList())}\nDescription Overview\n{string.Join("\n", commandDescriptionDictionary.Values.Select(description => $"{j++}. {description}").ToList())}";
    }
    
    public override bool IsExecuteOnClient => false;

    public override bool AllowedInMainMenu => true;

    public override string[] GetCommands() => new[]
    {
        "backup",
        "bp"
    };

    public override string GetDescription() =>
        "Some commands to simplify the creation of backups (commands are provided by BackupMod)";

    public override string GetHelp() => _helpText;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public override async void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        IReadOnlyList<string> args = _params.Select(arg => arg.ToLowerInvariant()).AsIReadOnlyList();
        
        _logger.LogDebug("The 'backup' command was invoked with params: '{Params}'", args);

        try
        {
            switch (args.Count)
            {
                case 0:
                    await BackupInternal();
                    OnCommandExecuted(new ConsoleCmdEventArgs(ConsoleCmdType.Backup, args));
                    
                    return;
                case 1:
                {
                    switch (args[0])
                    {
                        case "info":
                            BackupInfoInternal();
                            OnCommandExecuted(new ConsoleCmdEventArgs(ConsoleCmdType.BackupInfo, args));

                            break;
                        case "restore":
                            await BackupRestoreInternal(null, null, null);
                            OnCommandExecuted(new ConsoleCmdEventArgs(ConsoleCmdType.BackupRestore, args));

                            break;
                        case "delete":
                            await BackupDeleteInternal(null, null, null);
                            OnCommandExecuted(new ConsoleCmdEventArgs(ConsoleCmdType.BackupDelete, args));

                            break;
                        case "list":
                            BackupListInternal();
                            OnCommandExecuted(new ConsoleCmdEventArgs(ConsoleCmdType.BackupList, args));

                            break;
                        case "start":
                            BackupStartInternal();
                            OnCommandExecuted(new ConsoleCmdEventArgs(ConsoleCmdType.BackupStart, args));

                            break;
                        case "stop":
                            BackupStopInternal();
                            OnCommandExecuted(new ConsoleCmdEventArgs(ConsoleCmdType.BackupStop, args));

                            break;
                        default:
                            LogUnknownCommand();
                            break;
                    }

                    return;
                }
                case 4:
                {
                    switch (args[0])
                    {
                        case "restore":
                        {
                            if (!TryParseArguments(
                                    args[1], args[2], args[3],
                                    out int worldId, out int saveId, out int backupId)
                               )
                            {
                                return;
                            }

                            await BackupRestoreInternal(worldId, saveId, backupId);
                            OnCommandExecuted(new ConsoleCmdEventArgs(ConsoleCmdType.BackupRestore, args));

                            break;
                        }
                        case "delete":
                        {
                            if (!TryParseArguments(
                                    args[1], args[2], args[3],
                                    out int worldId, out int saveId, out int backupId)
                               )
                            {
                                return;
                            }

                            await BackupDeleteInternal(worldId, saveId, backupId);
                            OnCommandExecuted(new ConsoleCmdEventArgs(ConsoleCmdType.BackupDelete, args));

                            break;
                        }
                        default:
                            LogUnknownCommand();
                            break;
                    }

                    return;
                }
                default:
                    LogWrongArgumentsNumber();

                    return;
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected exception was occured during executing a command");
        }
    }
    
    private async Task BackupInternal()
    {
        if (_worldService.GetCurrentWorld() == null)
        {
            _logger.LogError("This command can only be executed when the game is started");

            return;
        }

        (BackupInfo backupInfo, TimeSpan timeElapsed) result =
            await _backupManager.CreateAsync("Manual backup", BackupMode.SaveAllAndBackup);

        _logger.LogInformation("The manual backup was successfully completed");
        _logger.LogInformation($"The backup file location: \"{result.backupInfo.Filepath}\"");

        _chatService?.SendMessage("The manual backup was successfully completed");
    }

    private bool TryParseArguments(string worldIdParam, string saveIdParam, string backupIdParam, out int worldId,
        out int saveId, out int backupId)
    {
        var valid = true;

        if (!int.TryParse(worldIdParam, out worldId))
        {
            LogInvalidArgument("world id", worldIdParam);

            valid = false;
        }

        if (!int.TryParse(saveIdParam, out saveId))
        {
            LogInvalidArgument("save id", saveIdParam);

            valid = false;
        }

        if (!int.TryParse(backupIdParam, out backupId))
        {
            LogInvalidArgument("backup id", backupIdParam);

            valid = false;
        }

        return valid;
    }
    
    private bool ValidateArguments(int? worldId, int? saveId, int? backupId)
    {
        if (worldId == null || saveId == null || backupId == null)
        {
            return false;
        }
        
        IReadOnlyList<WorldInfo> worlds = _worldInfoService.GetWorldInfos();
        
        int worldsCount = worlds.Count;
        if (worldId >= worldsCount || worldId < 0)
        {
            if (worldsCount != 0)
            {
                LogArgumentIsOutOfRange("world id", worldsCount - 1);
            }
            else
            {
                LogThereAreNoBackups();
            }

            return false;
        }

        int savesCount = worlds[worldId.Value].Saves.Count;
        if (saveId >= savesCount || saveId < 0)
        {
            if (savesCount != 0)
            {
                LogArgumentIsOutOfRange("save id", savesCount - 1);
            }
            else
            {
                LogThereAreNoBackups();
            }
            
            return false;
        }

        int backupsCount = worlds[worldId.Value].Saves[saveId.Value].Backups.Count;
        if (backupId >= backupsCount || backupId < 0)
        {
            if (backupsCount != 0)
            {
                LogArgumentIsOutOfRange("backup id", backupsCount - 1);
            }
            else
            {
                LogThereAreNoBackups();
            }
            
            return false;
        }
        
        return true;
    }

    private void LogInvalidArgument(string argumentName, string argumentValue) =>
        _logger.LogInformation("'{Value}' is an invalid {Name}", argumentValue, argumentName);

    private void LogUnknownCommand() =>
        _logger.LogInformation("Unknown command. Check that the command is entered correctly");

    private void LogWrongArgumentsNumber() => 
        _logger.LogInformation("Unknown command. Wrong number of arguments");

    private void LogThereAreNoBackups() =>
        _logger.LogInformation("There are no backups. First you should make a backup of any save");

    private void LogArgumentIsOutOfRange(string argumentName, int rangeMaxBorder) => 
        _logger.LogInformation(
        "This is an invalid {ArgumentName}. Please specify an existing {ArgumentName} in the range 0-{RangeMaxBorder}",
        argumentName, argumentName, rangeMaxBorder.ToString());
}