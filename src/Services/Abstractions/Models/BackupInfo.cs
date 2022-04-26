using System;

namespace NtakliveBackupMod.Services.Abstractions.Models;

public class BackupInfo
{
    public string Filepath { get; set; }
    public string WorldName { get; set; }
    public string SaveName { get; set; }
    public DateTime Timestamp { get; set; }
}