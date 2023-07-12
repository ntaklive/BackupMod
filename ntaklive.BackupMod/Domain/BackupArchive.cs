using System;
using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class BackupArchive : ValueObject<BackupArchive>
    {
        public BackupArchive(string? filepath)
        {
            if (filepath == null!)
            {
                throw new ArgumentNullException(nameof(filepath));
            }

            Filepath = filepath;
            Extension = filepath.Substring(filepath.IndexOf('.'), filepath.Length - filepath.IndexOf('.'));
        }
        
        public string Filepath { get; }
        
        public string Extension { get; }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new object[] {Filepath};
        }
    }
}