using System;
using System.Threading;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IBackupManager
{
    public Task<(BackupInfo backupInfo, TimeSpan timeElapsed)> CreateAsync(string title, BackupMode mode, CancellationToken token = default);

    public Task RestoreAsync(BackupInfo backupInfo, CancellationToken token = default);
    
    public Task DeleteAsync(BackupInfo backupInfo, CancellationToken token = default);
}