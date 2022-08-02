using BackupMod.DI;
using BackupMod.Services.Abstractions;

namespace BackupMod.FilUnderscore;

/// <summary>
/// FilUnderscore`s ModManager support <br/>
/// Important! Initialize the mod manager settings after the Bootstrapper.Initialize() method was invoked
/// </summary>
public static class ModManagerBootstrapper
{
    private static readonly IConfigurationService ConfigurationService = ServiceLocator.GetRequiredService<IConfigurationService>();
    private static readonly IDirectoryService DirectoryService = ServiceLocator.GetRequiredService<IDirectoryService>();

    private static int _generalBackupsLimit = Configuration.Default.General.BackupsLimit;
    private static string _generalCustomBackupsFolder = Configuration.Default.General.CustomBackupsFolder;
    private static bool _autoBackupEnabled = Configuration.Default.AutoBackup.Enabled;
    private static int _autoBackupDelay = Configuration.Default.AutoBackup.Delay;
    private static bool _autoBackupSkipIfThereIsNoPlayers = Configuration.Default.AutoBackup.SkipIfThereIsNoPlayers;
    private static bool _archiveEnabled = Configuration.Default.Archive.Enabled;
    private static int _archiveBackupsLimit = Configuration.Default.Archive.BackupsLimit;
    private static string _archiveCustomArchiveFolder = Configuration.Default.Archive.CustomArchiveFolder;
    private static bool _eventsBackupOnWorldLoaded = Configuration.Default.Events.BackupOnWorldLoaded;
    private static bool _eventsBackupOnServerIsEmpty = Configuration.Default.Events.BackupOnServerIsEmpty;
    private static bool _utilitiesChatNotificationsEnabled = Configuration.Default.Utilities.ChatNotificationsEnabled;

    public static void Initialize(Mod modInstance)
    {
        if (!ModManagerAPI.IsModManagerLoaded()) return;

        ModManagerAPI.ModSettings modSettings = ModManagerAPI.GetModSettings(modInstance);

        modSettings.CreateTab("general_tab", "general_tab");
        modSettings.CreateTab("autoBackup_tab", "autoBackup_tab");
        modSettings.CreateTab("archive_tab", "archive_tab");
        modSettings.CreateTab("events_tab", "events_tab");
        modSettings.CreateTab("utilities_tab", "utilities_tab");

        // General
        modSettings.Hook(
                "general_backupsLimit",
                "general_backupsLimit",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.General.BackupsLimit = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _generalBackupsLimit = value;
                },
                () => _generalBackupsLimit,
                value => (value.ToString(), value.ToString()),
                str =>
                {
                    bool success = int.TryParse(str, out int val);
                    return (val, success);
                })
            .SetTab("general_tab")
            .SetMinimumMaximumAndIncrementValues(1, 999, 1);

        modSettings.Hook(
                "general_customBackupsFolder",
                "general_customBackupsFolder",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.General.CustomBackupsFolder = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _generalCustomBackupsFolder = value;
                },
                () => _generalCustomBackupsFolder,
                value => (value.ToString(), value.ToString()),
                str =>
                {
                    bool success = string.IsNullOrEmpty(str) || DirectoryService.IsDirectoryExists(str);
                    return (str, success);
                })
            .SetTab("general_tab");

        // AutoBackup
        modSettings.Hook(
                "autoBackup_enabled",
                "autoBackup_enabled",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.AutoBackup.Enabled = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _autoBackupEnabled = value;
                },
                () => _autoBackupEnabled,
                value => (value.ToString(), value.ToString()),
                str => (bool.Parse(str), true))
            .SetTab("autoBackup_tab")
            .SetAllowedValues(new[] {false, true})
            .SetWrap(true);

        modSettings.Hook(
                "autoBackup_delay",
                "autoBackup_delay",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.AutoBackup.Delay = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _autoBackupDelay = value;
                },
                () => _autoBackupDelay,
                value => (value.ToString(), value.ToString()),
                str =>
                {
                    bool success = int.TryParse(str, out int val) && val is >= 10 and < 86400;
                    return (val, success);
                })
            .SetTab("autoBackup_tab");
        
        modSettings.Hook(
                "autoBackup_skipIfThereIsNoPlayers",
                "autoBackup_skipIfThereIsNoPlayers",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.AutoBackup.SkipIfThereIsNoPlayers = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _autoBackupSkipIfThereIsNoPlayers = value;
                },
                () => _autoBackupSkipIfThereIsNoPlayers,
                value => (value.ToString(), value.ToString()),
                str => (bool.Parse(str), true))
            .SetTab("autoBackup_tab")
            .SetAllowedValues(new[] {false, true})
            .SetWrap(true);

        // Archive
        modSettings.Hook(
                "archive_enabled",
                "archive_enabled",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.Archive.Enabled = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _archiveEnabled = value;
                },
                () => _archiveEnabled,
                value => (value.ToString(), value.ToString()),
                str => (bool.Parse(str), true))
            .SetTab("archive_tab")
            .SetAllowedValues(new[] {false, true})
            .SetWrap(true);

        modSettings.Hook(
                "archive_backupsLimit",
                "archive_backupsLimit",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.Archive.BackupsLimit = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _archiveBackupsLimit = value;
                },
                () => _archiveBackupsLimit,
                value => (value.ToString(), value.ToString()),
                str =>
                {
                    bool success = int.TryParse(str, out int val);
                    return (val, success);
                })
            .SetTab("archive_tab")
            .SetMinimumMaximumAndIncrementValues(1, 999, 1);

        modSettings.Hook(
                "archive_customArchiveFolder",
                "archive_customArchiveFolder",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.Archive.CustomArchiveFolder = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _archiveCustomArchiveFolder = value;
                },
                () => _archiveCustomArchiveFolder,
                value => (value.ToString(), value.ToString()),
                str =>
                {
                    bool success = string.IsNullOrEmpty(str) || DirectoryService.IsDirectoryExists(str);
                    return (str, success);
                })
            .SetTab("archive_tab");

        // Events
        modSettings.Hook(
                "events_backupOnWorldLoaded",
                "events_backupOnWorldLoaded",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.Events.BackupOnWorldLoaded = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _eventsBackupOnWorldLoaded = value;
                },
                () => _eventsBackupOnWorldLoaded,
                value => (value.ToString(), value.ToString()),
                str => (bool.Parse(str), true))
            .SetTab("events_tab")
            .SetAllowedValues(new[] {false, true})
            .SetWrap(true);
        
        modSettings.Hook(
                "events_backupOnServerIsEmpty",
                "events_backupOnServerIsEmpty",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.Events.BackupOnServerIsEmpty = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _eventsBackupOnServerIsEmpty = value;
                },
                () => _eventsBackupOnServerIsEmpty,
                value => (value.ToString(), value.ToString()),
                str => (bool.Parse(str), true))
            .SetTab("events_tab")
            .SetAllowedValues(new[] {false, true})
            .SetWrap(true);

        // Utilities
        modSettings.Hook(
                "utilities_chatNotificationsEnabled",
                "utilities_chatNotificationsEnabled",
                value =>
                {
                    Configuration configuration = ConfigurationService.GetConfiguration();

                    configuration.Utilities.ChatNotificationsEnabled = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _utilitiesChatNotificationsEnabled = value;
                },
                () => _utilitiesChatNotificationsEnabled,
                value => (value.ToString(), value.ToString()),
                str => (bool.Parse(str), true))
            .SetTab("utilities_tab")
            .SetAllowedValues(new[] {false, true})
            .SetWrap(true);
    }
}