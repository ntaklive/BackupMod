using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Infrastructure
{
    public interface IGameSaveFactory
    {
        public Result<IList<GameSave>> LoadAvailableSaves(GameWorld world);
    }
}