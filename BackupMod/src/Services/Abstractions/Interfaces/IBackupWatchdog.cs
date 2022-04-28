using System;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IBackupWatchdog
{
    public Task StartAsync(World world, SaveInfo saveInfo, TimeSpan delay, BackupMode backupMode);
}