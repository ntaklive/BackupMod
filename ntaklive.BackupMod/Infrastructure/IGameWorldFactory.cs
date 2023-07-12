using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Infrastructure
{
    public interface IGameWorldFactory
    {
        Result<IList<GameWorld>> LoadAvailableWorlds();
    }
}