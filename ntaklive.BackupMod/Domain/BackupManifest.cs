using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class BackupManifest : IMappable<Backup, BackupManifest>
    {
        public BackupManifest(
            string title,
            string backupCaller,
            string archiveExtension,
            string saveName,
            string worldName,
            string worldHash,
            int gameTimeDay,
            int gameTimeHour,
            int gameTimeMinute,
            int realDateDay,
            int realDateMonth,
            int realDateYear,
            int realTimeHour,
            int realTimeMinute,
            int realTimeSecond)
        {
            Title = title;
            BackupCaller = backupCaller;
            ArchiveExtension = archiveExtension;
            SaveName = saveName;
            WorldName = worldName;
            WorldHash = worldHash;
            GameTimeDay = gameTimeDay;
            GameTimeHour = gameTimeHour;
            GameTimeMinute = gameTimeMinute;
            RealDateDay = realDateDay;
            RealDateMonth = realDateMonth;
            RealDateYear = realDateYear;
            RealTimeHour = realTimeHour;
            RealTimeMinute = realTimeMinute;
            RealTimeSecond = realTimeSecond;
        }

        public string Title { get; }
        public string BackupCaller { get; }
        public string ArchiveExtension { get; }
    
        public string SaveName { get; }
    
        public string WorldName { get; }
        public string WorldHash { get; }

        public int GameTimeDay { get; }
        public int GameTimeHour { get; }
        public int GameTimeMinute { get; }
    
        public int RealDateDay { get; }
        public int RealDateMonth { get; }
        public int RealDateYear { get; }
    
        public int RealTimeHour { get; }
        public int RealTimeMinute { get; }
        public int RealTimeSecond { get; }

        /// <summary>
        /// This should be used only for invoking the Map method
        /// </summary>
        public static readonly BackupManifest Empty = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    
        public BackupManifest Map(Backup backup)
        {
            return new BackupManifest(
                title: backup.Info.Title,
                backupCaller: backup.Info.Caller.ToString(),
                archiveExtension: backup.Archive.Extension,
                saveName: backup.Save.Name,
                worldName: backup.Save.World.Name,
                worldHash: backup.Save.World.Hash,
                gameTimeDay: backup.Info.Time.GameTime.Day,
                gameTimeHour: backup.Info.Time.GameTime.Hour,
                gameTimeMinute: backup.Info.Time.GameTime.Minute,
                realDateDay: backup.Info.Time.RealTime.Date.Day,
                realDateMonth: backup.Info.Time.RealTime.Date.Month,
                realDateYear: backup.Info.Time.RealTime.Date.Year,
                realTimeHour: backup.Info.Time.RealTime.Time.Hour,
                realTimeMinute: backup.Info.Time.RealTime.Time.Minute,
                realTimeSecond: backup.Info.Time.RealTime.Time.Second);
        }
    }
}