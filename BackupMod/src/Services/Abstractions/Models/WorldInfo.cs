namespace BackupMod.Services.Abstractions.Models;

public class WorldInfo
{
    public string WorldFolderPath { get; set; }
    
    public string BackupsFolderPath { get; set; }
    
    public string ArchiveFolderPath { get; set; }

    public string WorldName { get; set; }
    
    public SaveInfo[] Saves { get; set; }
}