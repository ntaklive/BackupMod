using System;
using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class BackupInfo : ValueObject<BackupInfo>
    {
        public BackupInfo(string? title, BackupCaller? backupCaller, BackupTime? backupTime)
        {
            if (title == null!)
            {
                throw new ArgumentNullException(nameof(title));
            }
            if (backupTime == null!)
            {
                throw new ArgumentNullException(nameof(backupTime));
            }
            if (backupCaller == null!)
            {
                throw new ArgumentNullException(nameof(backupCaller));
            }

            Title = title;
            Caller = backupCaller.Value;
            Time = backupTime;
        }
        
        public string Title { get; }
        
        public BackupCaller Caller { get; }
        
        public BackupTime Time { get; }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new object[] {Caller.ToString(), Time};
        }
    }
}