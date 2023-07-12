namespace BackupMod.Services.Abstractions;

public interface IResources
{
    public string GetBackupsDirectoryPath();

    public string GetArchiveDirectoryPath();
    
    public string GetSavesDirectoryPath();

    public string GetWorldsDirectoryPath();

    public string GetMd5HashForWorld(string worldName);
}