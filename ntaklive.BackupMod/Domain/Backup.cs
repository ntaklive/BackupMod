using System;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class Backup : AggregateRoot
    {
        public Backup(GameSave? gameSave, BackupInfo? backupInfo, BackupArchive? backupArchive)
        {
            if (gameSave! == null!)
            {
                throw new ArgumentNullException(nameof(gameSave));
            }            
            if (backupInfo == null!)
            {
                throw new ArgumentNullException(nameof(backupInfo));
            }
            if (backupArchive == null!)
            {
                throw new ArgumentNullException(nameof(backupArchive));
            }
            
            Save = gameSave;
            Info = backupInfo;
            Archive = backupArchive;
        }
        
        public GameSave Save { get; }
        
        public BackupInfo Info { get; }
        
        public BackupArchive Archive { get; }

        public void Restore()
        {
            // AddDomainEvent(new RestoreRequestedDomainEvent(this));
        }
    }
}