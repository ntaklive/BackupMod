using BackupMod.Manifest;

namespace BackupMod.Services.Abstractions;

public interface IBackupManifestService
{
    public BackupManifest ReadManifest(string filepath);

    public void CreateManifest(string filepath, BackupManifest manifest);
}