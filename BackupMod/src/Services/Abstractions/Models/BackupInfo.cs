using System;

namespace BackupMod.Services.Abstractions.Models;

public class BackupInfo
{
    public string Filepath { get; set; }
    
    public SaveInfo SaveInfo { get; set; }
    
    public DateTime Timestamp { get; set; }
}