using System;
using System.Threading.Tasks;
using NtakliveBackupMod.Services.Abstractions.Enum;
using NtakliveBackupMod.Services.Abstractions.Models;

namespace NtakliveBackupMod.Services.Abstractions;

public interface IBackupWatchdog
{
    public Task Start(World world, SaveInfo saveInfo, TimeSpan delay, BackupMode backupMode);
}