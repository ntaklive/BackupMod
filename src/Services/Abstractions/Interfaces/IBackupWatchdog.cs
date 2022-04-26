using System;
using System.Threading.Tasks;
using NtakliveBackupMod.Scripts.Services.Abstractions.Enum;
using NtakliveBackupMod.Scripts.Services.Abstractions.Models;

namespace NtakliveBackupMod.Scripts.Services.Abstractions;

public interface IBackupWatchdog
{
    public Task Start(World world, SaveInfo saveInfo, TimeSpan delay, BackupMode backupMode);
}