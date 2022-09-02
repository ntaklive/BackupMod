using System;
using BackupMod.Modules.Base;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;

namespace BackupMod.Modules.FilUnderscore;

public sealed class FilUnderscoreModule : ModuleBase
{
    private int _generalBackupsLimit = ModConfiguration.Default.General.BackupsLimit;
    private string _generalCustomBackupsFolder = ModConfiguration.Default.General.CustomBackupsFolder;
    private bool _generalDebugMode = ModConfiguration.Default.General.DebugMode;

    private bool _autoBackupEnabled = ModConfiguration.Default.AutoBackup.Enabled;
    private int _autoBackupDelay = ModConfiguration.Default.AutoBackup.Delay;
    private bool _autoBackupSkipIfThereAreNoPlayers = ModConfiguration.Default.AutoBackup.SkipIfThereAreNoPlayers;

    private bool _archiveEnabled = ModConfiguration.Default.Archive.Enabled;
    private int _archiveBackupsLimit = ModConfiguration.Default.Archive.BackupsLimit;
    private string _archiveCustomArchiveFolder = ModConfiguration.Default.Archive.CustomArchiveFolder;

    private bool _eventsBackupOnWorldLoaded = ModConfiguration.Default.Events.BackupOnWorldLoaded;
    private bool _eventsBackupOnServerIsEmpty = ModConfiguration.Default.Events.BackupOnServerIsEmpty;

    private bool _notificationsEnabled = ModConfiguration.Default.Notifications.Enabled;
    private bool _notificationsCountdownEnabled = ModConfiguration.Default.Notifications.Countdown.Enabled;
    private int _notificationsCountdownCountFrom = ModConfiguration.Default.Notifications.Countdown.CountFrom;

    private IConfigurationService ConfigurationService { get; set; }
    private IDirectoryService DirectoryService { get; set; }

    public override void InitializeModule(IServiceProvider provider)
    {
        if (!ModManagerAPI.IsModManagerLoaded()) return;

        ConfigurationService = ServiceProviderExtensions.GetRequiredService<IConfigurationService>(provider);
        DirectoryService = ServiceProviderExtensions.GetRequiredService<IDirectoryService>(provider);

        var mod = ServiceProviderExtensions.GetRequiredService<Mod>(provider);

        ConfigureHooks(mod);
    }

    private void ConfigureHooks(Mod mod)
    {
        ModManagerAPI.ModSettings modSettings = ModManagerAPI.GetModSettings(mod);
        
        // Tabs
        modSettings.CreateTab("general_tab", "general_tab");
        modSettings.CreateTab("autoBackup_tab", "autoBackup_tab");
        modSettings.CreateTab("archive_tab", "archive_tab");
        modSettings.CreateTab("events_tab", "events_tab");
        modSettings.CreateTab("notifications_tab", "notifications_tab");

        // General
        modSettings.Hook(
                "general_backupsLimit",
                "general_backupsLimit",
                value =>
                {
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

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
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

                    configuration.General.CustomBackupsFolder = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _generalCustomBackupsFolder = value;
                },
                () => _generalCustomBackupsFolder,
                value => (value.ToString(), value.ToString()),
                str =>
                {
                    bool success = string.IsNullOrEmpty(str) || DirectoryService.Exists(str);
                    return (str, success);
                })
            .SetTab("general_tab");
        
        modSettings.Hook(
                "general_debugMode",
                "general_debugMode",
                value =>
                {
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

                    configuration.General.DebugMode = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _generalDebugMode = value;
                },
                () => _generalDebugMode,
                value => (value.ToString(), value.ToString()),
                str => (bool.Parse(str), true))
            .SetTab("general_tab")
            .SetAllowedValues(new[] {false, true})
            .SetWrap(true);

        // AutoBackup
        modSettings.Hook(
                "autoBackup_enabled",
                "autoBackup_enabled",
                value =>
                {
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

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
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

                    configuration.AutoBackup.Delay = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _autoBackupDelay = value;
                },
                () => _autoBackupDelay,
                value => (value.ToString(), value.ToString()),
                str =>
                {
                    bool success = int.TryParse(str, out int val);
                    return (val, success);
                })
            .SetTab("autoBackup_tab")
            .SetMinimumMaximumAndIncrementValues(10, 86400, 10);

        modSettings.Hook(
                "autoBackup_skipIfThereAreNoPlayers",
                "autoBackup_skipIfThereAreNoPlayers",
                value =>
                {
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

                    configuration.AutoBackup.SkipIfThereAreNoPlayers = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _autoBackupSkipIfThereAreNoPlayers = value;
                },
                () => _autoBackupSkipIfThereAreNoPlayers,
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
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

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
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

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
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

                    configuration.Archive.CustomArchiveFolder = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _archiveCustomArchiveFolder = value;
                },
                () => _archiveCustomArchiveFolder,
                value => (value.ToString(), value.ToString()),
                str =>
                {
                    bool success = string.IsNullOrEmpty(str) || DirectoryService.Exists(str);
                    return (str, success);
                })
            .SetTab("archive_tab");

        // Events
        modSettings.Hook(
                "events_backupOnWorldLoaded",
                "events_backupOnWorldLoaded",
                value =>
                {
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

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
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

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

        // Notifications
        modSettings.Hook(
                "notifications_enabled",
                "notifications_enabled",
                value =>
                {
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

                    configuration.Notifications.Enabled = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _notificationsEnabled = value;
                },
                () => _notificationsEnabled,
                value => (value.ToString(), value.ToString()),
                str => (bool.Parse(str), true))
            .SetTab("notifications_tab")
            .SetAllowedValues(new[] {false, true})
            .SetWrap(true);

        // Notifications ---> Countdown
        modSettings.Category("notificationsCountdown_category", "notificationsCountdown_category").SetTab("notifications_tab");

        modSettings.Hook(
                "notifications_countdownEnabled",
                "notifications_countdownEnabled",
                value =>
                {
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

                    configuration.Notifications.Countdown.Enabled = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _notificationsCountdownEnabled = value;
                },
                () => _notificationsCountdownEnabled,
                value => (value.ToString(), value.ToString()),
                str => (bool.Parse(str), true))
            .SetTab("notifications_tab")
            .SetAllowedValues(new[] {false, true})
            .SetWrap(true);
        
        modSettings.Hook(
                "notifications_countdownCountFrom",
                "notifications_countdownCountFrom",
                value =>
                {
                    ModConfiguration configuration = ConfigurationService.ReadConfiguration();

                    configuration.Notifications.Countdown.CountFrom = value;

                    ConfigurationService.TryUpdateConfiguration(configuration);

                    _notificationsCountdownCountFrom = value;
                },
                () => _notificationsCountdownCountFrom,
                value => (value.ToString(), value.ToString()),
                str =>
                {
                    bool success = int.TryParse(str, out int val);
                    return (val, success);
                })
            .SetTab("notifications_tab")
            .SetMinimumMaximumAndIncrementValues(1, 999, 1);
    }
}