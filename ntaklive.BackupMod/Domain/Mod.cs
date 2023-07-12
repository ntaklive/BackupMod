using System;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public sealed class Mod
    {
        public const string BackupManifestExtension = ".manifest.json";

        public Mod(Configuration configuration)
        {
            Configuration = configuration;
        }

        public Configuration Configuration { get; private set; }

        public string BackupsDirectoryPath => Configuration.General.BackupsDirectoryPath;

        public string ArchiveDirectoryPath => Configuration.Archive.ArchiveDirectoryPath;

        public void UpdateConfiguration(Configuration? configuration)
        {
            if (configuration! == null!)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
        
            Configuration = configuration;
        }
    
        public string GetBackupDirectoryPath(string worldName, string worldHash, string saveName)
        {
            return Filesystem.Path.Combine(BackupsDirectoryPath, worldName, worldHash, saveName);
        }    
    
        public string GetArchiveDirectoryPath(string worldName, string worldHash, string saveName)
        {
            return Filesystem.Path.Combine(ArchiveDirectoryPath, worldName, worldHash, saveName);
        }
    }
}