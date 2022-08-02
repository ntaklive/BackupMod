using System.Diagnostics.CodeAnalysis;

namespace BackupMod;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Configuration
{
    public GeneralConfiguration General { get; set; }
    public AutoBackupConfiguration AutoBackup { get; set; }
    public ArchiveConfiguration Archive { get; set; }
    public EventsConfiguration Events { get; set; }
    public UtilitiesConfiguration Utilities { get; set; }

    public static readonly Configuration Default = new()
    {
        General = new GeneralConfiguration(backupsLimit: 10, customBackupsFolder: ""),
        AutoBackup = new AutoBackupConfiguration(enabled: true, delay: 1200, skipIfThereIsNoPlayers: false),
        Archive = new ArchiveConfiguration(enabled: false, backupsLimit: 10, customArchiveFolder: ""),
        Events = new EventsConfiguration(backupOnWorldLoaded: true, backupOnServerIsEmpty: false),
        Utilities = new UtilitiesConfiguration(chatNotificationsEnabled: true)
    };

    public sealed class GeneralConfiguration
    {
        public GeneralConfiguration()
        {
        }

        public GeneralConfiguration(int backupsLimit, string customBackupsFolder)
        {
            BackupsLimit = backupsLimit;
            CustomBackupsFolder = customBackupsFolder;
        }

        public int BackupsLimit { get; set; }
        public string CustomBackupsFolder { get; set; }
    }

    public class AutoBackupConfiguration
    {
        public AutoBackupConfiguration()
        {
        }

        public AutoBackupConfiguration(bool enabled, int delay, bool skipIfThereIsNoPlayers)
        {
            Enabled = enabled;
            Delay = delay;
            SkipIfThereIsNoPlayers = skipIfThereIsNoPlayers;
        }

        public bool Enabled { get; set; }
        public int Delay { get; set; }
        public bool SkipIfThereIsNoPlayers { get; set; }
    }

    public class ArchiveConfiguration
    {
        public ArchiveConfiguration()
        {
        }

        public ArchiveConfiguration(bool enabled, int backupsLimit, string customArchiveFolder)
        {
            Enabled = enabled;
            BackupsLimit = backupsLimit;
            CustomArchiveFolder = customArchiveFolder;
        }

        public bool Enabled { get; set; }
        public int BackupsLimit { get; set; }
        public string CustomArchiveFolder { get; set; }
    }

    public class EventsConfiguration
    {
        public EventsConfiguration()
        {
        }

        public EventsConfiguration(bool backupOnWorldLoaded, bool backupOnServerIsEmpty)
        {
            BackupOnWorldLoaded = backupOnWorldLoaded;
            BackupOnServerIsEmpty = backupOnServerIsEmpty;
        }

        public bool BackupOnWorldLoaded { get; set; }
        public bool BackupOnServerIsEmpty { get; set; }
    }

    public class UtilitiesConfiguration
    {
        public UtilitiesConfiguration()
        {
        }

        public UtilitiesConfiguration(bool chatNotificationsEnabled)
        {
            ChatNotificationsEnabled = chatNotificationsEnabled;
        }

        public bool ChatNotificationsEnabled { get; set; }
    }
}