using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Infrastructure
{
    public interface IBackupService
    {
        public Result<IList<Backup>> GetAvailableBackups(string? worldName, string? worldHash, string? saveName);
    }
}