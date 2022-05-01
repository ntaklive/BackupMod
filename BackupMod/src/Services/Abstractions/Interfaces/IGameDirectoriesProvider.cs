namespace BackupMod.Services.Abstractions;

public interface IGameDirectoriesProvider
{
    public string GetBackupsFolderPath();
    
    public string GetSavesFolderPath();
}