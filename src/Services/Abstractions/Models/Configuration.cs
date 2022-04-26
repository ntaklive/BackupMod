namespace NtakliveBackupMod.Scripts.Services.Abstractions.Models;

public class Configuration
{
    public int AutoBackupDelay { get; set; }
    public int BackupsLimit { get; set; }

    public bool EnableChatMessages { get; set; }

    public bool BackupOnWorldLoaded { get; set; }

    public string CustomBackupsFolder { get; set; }
}