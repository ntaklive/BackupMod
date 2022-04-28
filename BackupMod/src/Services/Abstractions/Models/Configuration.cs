namespace BackupMod.Services.Abstractions.Models;

public class Configuration
{
    public int AutoBackupDelay { get; set; }
    public int BackupsLimit { get; set; }

    public bool EnableChatMessages { get; set; }

    public bool BackupOnWorldLoaded { get; set; }

    public string CustomBackupsFolder { get; set; }

    public static readonly Configuration Default = new()
    {
        AutoBackupDelay = 600,
        BackupsLimit = 5,
        EnableChatMessages = true,
        BackupOnWorldLoaded = true,
        CustomBackupsFolder = ""
    };
}