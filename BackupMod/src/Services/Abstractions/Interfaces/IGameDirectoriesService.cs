using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IGameDirectoriesService
{
    public string GetBackupsFolderPath();

    public string GetArchiveFolderPath();
    
    public string GetSavesFolderPath();

    public void CreateRequiredFolders(SaveInfo saveInfo);
}