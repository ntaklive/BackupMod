namespace BackupMod.Services.Abstractions.Models;

public class SaveInfo
{
    public string SaveName { get; set; }
    
    public string SaveFolderPath { get; set; }
    
    public string BackupsFolderPath { get; set; }
    
    public string ArchiveFolderPath { get; set; }

    public WorldInfo World { get; set; }

    public BackupInfo[] Backups { get; set; }
}