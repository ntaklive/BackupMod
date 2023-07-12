using BackupMod.Manifest;

namespace BackupMod.Services.Abstractions;

public interface IBackupManifestFactory
{
    public BackupManifest Create(string title, string backupFilename, string worldName, string saveName);
}