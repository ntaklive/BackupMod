using System;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Infrastructure
{
    public interface IGameSaveBackingUpAlgorithm
    {
        public ValueResult<(Backup backup, TimeSpan timeSpent)> BackUpActiveSave(string? title, BackupCaller? caller, bool? saveBeforeBackingUp = true);
    }
}