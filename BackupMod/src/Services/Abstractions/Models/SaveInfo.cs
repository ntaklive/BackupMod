using System.Collections.Generic;

namespace BackupMod.Services.Abstractions.Models;

public class SaveInfo
{
    public string Name { get; set; }

    public string DirectoryPath { get; set; }
    
    public IReadOnlyList<BackupInfo> Backups { get; set; }
}