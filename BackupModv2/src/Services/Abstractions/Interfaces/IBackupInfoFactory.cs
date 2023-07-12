using BackupMod.Manifest;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IBackupInfoFactory
{
    public BackupInfo CreateFromManifest(BackupManifest manifest);
}