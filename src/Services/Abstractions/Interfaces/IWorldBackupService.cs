using NtakliveBackupMod.Services.Abstractions.Enum;
using NtakliveBackupMod.Services.Abstractions.Models;

namespace NtakliveBackupMod.Services.Abstractions;

public interface IWorldBackupService
{
    public string Backup(SaveInfo saveInfo, BackupMode mode);
}