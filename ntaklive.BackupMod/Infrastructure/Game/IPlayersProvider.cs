namespace ntaklive.BackupMod.Infrastructure.Game
{
    public interface IPlayersProvider
    {
        public PersistentPlayerList GetCurrentPersistentPlayerList();
        public PersistentPlayerData GetPersistentLocalPlayer();
    }
}