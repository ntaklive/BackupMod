using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Infrastructure
{
    public interface IGameFactory
    {
        public Result<Domain.Game> CreateGame();
    }
}