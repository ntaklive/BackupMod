using NtakliveBackupMod.Scripts.Services.Abstractions.Enum;
using NtakliveBackupMod.Scripts.Services.Abstractions.Models;

namespace NtakliveBackupMod.Scripts.Services.Abstractions;

public interface IWorldBackupService
{
    public string Backup(SaveInfo saveInfo, BackupMode mode);
}