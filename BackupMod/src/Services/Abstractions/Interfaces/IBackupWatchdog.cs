using System;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IBackupWatchdog
{
    public Task Start(World world, SaveInfo saveInfo, TimeSpan delay, BackupMode backupMode);
}