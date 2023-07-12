using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public sealed class Configuration : ValueObject<Configuration>
    {
        public Configuration(
            GeneralConfiguration generalConfiguration,
            AutoBackupConfiguration autoBackupConfiguration,
            ArchiveConfiguration archiveConfiguration,
            EventsConfiguration eventsConfiguration,
            NotificationsConfiguration notificationsConfiguration)
        {
            General = generalConfiguration;
            AutoBackup = autoBackupConfiguration;
            Archive = archiveConfiguration;
            Events = eventsConfiguration;
            Notifications = notificationsConfiguration;
        }

        public GeneralConfiguration General { get; }

        public AutoBackupConfiguration AutoBackup { get; }

        public ArchiveConfiguration Archive { get; }

        public EventsConfiguration Events { get; }

        public NotificationsConfiguration Notifications { get; }

        public static readonly Configuration Default = new(
            new GeneralConfiguration(backupsLimit: 10, backupsDirectoryPath: "", debugMode: false, backupArchiveExtension: ".zip"),
            new AutoBackupConfiguration(enabled: true, delay: 1200, skipIfThereAreNoPlayers: false, resetDelayTimerAfterManualBackup: false),
            new ArchiveConfiguration(enabled: false, backupsLimit: 10, archiveDirectoryPath: ""),
            new EventsConfiguration(backupOnWorldLoaded: true, backupOnServerIsEmpty: false),
            new NotificationsConfiguration(enabled: true, countdownConfiguration: 
                new NotificationsConfiguration.CountdownConfiguration(enabled: true, countFrom: 5))
        );

        public sealed class ArchiveConfiguration : ValueObject<ArchiveConfiguration>
        {
            public ArchiveConfiguration(bool enabled, int backupsLimit, string archiveDirectoryPath)
            {
                Enabled = enabled;
                BackupsLimit = backupsLimit;
                ArchiveDirectoryPath = archiveDirectoryPath;
            }

            public bool Enabled { get; }
            public int BackupsLimit { get; }
            public string ArchiveDirectoryPath { get; }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                return new object[] {Enabled, BackupsLimit, ArchiveDirectoryPath};
            }
        }

        public sealed class AutoBackupConfiguration : ValueObject<AutoBackupConfiguration>
        {
            public AutoBackupConfiguration(bool enabled, int delay, bool skipIfThereAreNoPlayers,
                bool resetDelayTimerAfterManualBackup)
            {
                Enabled = enabled;
                Delay = delay;
                ResetDelayTimerAfterManualBackup = resetDelayTimerAfterManualBackup;
                SkipIfThereAreNoPlayers = skipIfThereAreNoPlayers;
            }

            public bool Enabled { get; }
            public int Delay { get; }
            public bool ResetDelayTimerAfterManualBackup { get; }
            public bool SkipIfThereAreNoPlayers { get; }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                return new object[] {Enabled, Delay, ResetDelayTimerAfterManualBackup, SkipIfThereAreNoPlayers};
            }
        }

        public sealed class EventsConfiguration : ValueObject<EventsConfiguration>
        {
            public EventsConfiguration(bool backupOnWorldLoaded, bool backupOnServerIsEmpty)
            {
                BackupOnWorldLoaded = backupOnWorldLoaded;
                BackupOnServerIsEmpty = backupOnServerIsEmpty;
            }

            public bool BackupOnWorldLoaded { get; }
            public bool BackupOnServerIsEmpty { get; }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                return new object[] {BackupOnWorldLoaded, BackupOnServerIsEmpty};
            }
        }

        public sealed class GeneralConfiguration : ValueObject<GeneralConfiguration>
        {
            public GeneralConfiguration(int backupsLimit, string backupsDirectoryPath, bool debugMode, string backupArchiveExtension)
            {
                BackupsLimit = backupsLimit;
                BackupsDirectoryPath = backupsDirectoryPath;
                DebugMode = debugMode;
                BackupArchiveExtension = backupArchiveExtension;
            }

            public int BackupsLimit { get; }
            public string BackupsDirectoryPath { get; }
            public bool DebugMode { get; }
            public string BackupArchiveExtension { get; }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                return new object[] {BackupsLimit, BackupsDirectoryPath, DebugMode, BackupArchiveExtension};
            }
        }

        public class NotificationsConfiguration : ValueObject<NotificationsConfiguration>
        {
            public NotificationsConfiguration(bool enabled, CountdownConfiguration countdownConfiguration)
            {
                Enabled = enabled;

                Countdown = countdownConfiguration;
            }

            public bool Enabled { get; }

            public CountdownConfiguration Countdown { get; }

            public class CountdownConfiguration : ValueObject<CountdownConfiguration>
            {
                public CountdownConfiguration(bool enabled, int countFrom)
                {
                    Enabled = enabled;
                    CountFrom = countFrom;
                }

                public bool Enabled { get; }
                public int CountFrom { get; }

                protected override IEnumerable<object> GetEqualityComponents()
                {
                    return new object[] {Enabled, CountFrom};
                }
            }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                return new object[] {Enabled, Countdown};
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new object[] {General, AutoBackup, Archive, Events, Notifications};
        }
    }
}