using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NtakliveBackupMod.Scripts.DI;
using NtakliveBackupMod.Scripts.Services.Abstractions;
using NtakliveBackupMod.Scripts.Services.Abstractions.Enum;
using NtakliveBackupMod.Scripts.Services.Abstractions.Models;

namespace NtakliveBackupMod.Scripts.Commands;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ConsoleCmdBackup : ConsoleCmdAbstract
{
    private readonly IWorldBackupService _backupService = ServiceLocator.GetRequiredService<IWorldBackupService>();
    private readonly IWorldService _worldService = ServiceLocator.GetRequiredService<IWorldService>();
    private readonly IChatService _chatService = ServiceLocator.GetRequiredService<IChatService>();
    private readonly ILogger<ConsoleCmdBackup> _logger = ServiceLocator.GetRequiredService<ILogger<ConsoleCmdBackup>>();

    public override bool IsExecuteOnClient => false;
    
    public override bool AllowedInMainMenu => false;

    public override string[] GetCommands() => new string[2]
    {
        "backup",
        "bp"
    };

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        SaveInfo saveInfo = _worldService.GetCurrentWorldSaveInfo();
        
        _backupService.Backup(saveInfo, BackupMode.SaveAllAndBackup);
        
        _logger.Debug("The manual backup was successfully completed.");
        _chatService?.SendMessage("The manual backup was successfully completed.");
    }

    public override string GetDescription() => "make a backup of the current save (command from the BackupMod)";
}