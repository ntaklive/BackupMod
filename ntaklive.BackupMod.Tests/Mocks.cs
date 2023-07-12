using System.Reflection;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Tests
{
    public static class Mocks
    {
        static Mocks()
        {
            CreateDirectory(BaseDirectoryPath);
            CreateDirectory(BackupsDirectoryPath);
            CreateDirectory(FakeWorldDirectoryPath);
            CreateDirectory(FakeSaveDirectoryPath);
            
            CreateFile(FakeBackupFilepath);

            GameWorld = new(FakeWorldName, FakeWorldDirectoryPath, FakeWorldHash);
            GameSave = new (FakeSaveName, FakeSaveDirectoryPath, GameWorld);
            Backup = new (GameSave, new BackupInfo(FakeBackupName, BackupCaller.Event, BackupTime.Now(new GameTime(1,0,0))), new BackupArchive(FakeBackupFilepath));
        }

        public static readonly string BaseDirectoryName = "mocks";
        public static readonly string BaseDirectoryPath = Filesystem.Path.Combine(Assembly.GetCallingAssembly().Location.Replace(".dll", ""), BaseDirectoryName);
        
        public static readonly string BackupsDirectoryName = "Backups";
        public static readonly string BackupsDirectoryPath = Filesystem.Path.Combine(BackupsDirectoryName, BackupsDirectoryName);

        public static readonly string FakeWorldName = "Fake World";
        public static readonly string FakeWorldHash = "FakeHash";
        public static readonly string FakeWorldDirectoryPath = Filesystem.Path.Combine(BackupsDirectoryPath, FakeWorldName);
        
        public static readonly string FakeSaveName = "Fake Save";
        public static readonly string FakeSaveDirectoryPath = Filesystem.Path.Combine(FakeWorldDirectoryPath, FakeSaveName);

        public static readonly string FakeBackupName = "Fake Backup";
        public static readonly string FakeBackupFilename = "Fake Backup.zip";
        public static readonly string FakeBackupFilepath = Filesystem.Path.Combine(FakeSaveDirectoryPath, FakeBackupFilename);
        
        public static readonly GameWorld GameWorld;
        public static readonly GameSave GameSave;
        public static readonly Backup Backup;

        private static void CreateDirectory(string path) => Filesystem.Directory.Create(path);
        
        private static void CreateFile(string path) => Filesystem.File.WriteAllText(path, string.Empty);
    }
}