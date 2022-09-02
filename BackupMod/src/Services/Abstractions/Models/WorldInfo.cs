using System.Collections.Generic;

namespace BackupMod.Services.Abstractions.Models;

public class WorldInfo
{
    public string Name { get; set; }
    
    public string Md5Hash { get; set; }
    
    public IReadOnlyList<SaveInfo> Saves { get; set; }
    
    public string WorldDirectoryPath { get; set; }
}