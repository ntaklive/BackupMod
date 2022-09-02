namespace BackupMod;

public class ModConfiguration
{
    public GeneralConfiguration General { get; set; }
    
    public AutoBackupConfiguration AutoBackup { get; set; }
    
    public ArchiveConfiguration Archive { get; set; }
    
    public EventsConfiguration Events { get; set; }
    
    public NotificationsConfiguration Notifications { get; set; }

    public static readonly ModConfiguration Default = new()
    {
        General = new GeneralConfiguration(backupsLimit: 10, customBackupsFolder: "", debugMode: false),
        AutoBackup = new AutoBackupConfiguration(enabled: true, delay: 1200, skipIfThereAreNoPlayers: false),
        Archive = new ArchiveConfiguration(enabled: false, backupsLimit: 10, customArchiveFolder: ""),
        Events = new EventsConfiguration(backupOnWorldLoaded: true, backupOnServerIsEmpty: false),
        Notifications = new NotificationsConfiguration(enabled: true, countdownConfiguration: new NotificationsConfiguration.CountdownConfiguration(enabled: true, countFrom: 5))
    };
    
    public static class Constants
    {
        public static string BackupManifestExtension => ".manifest.json";
        public static string BackupArchiveExtension => ".zip";
    }
}