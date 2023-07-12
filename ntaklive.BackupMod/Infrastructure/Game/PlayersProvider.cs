namespace ntaklive.BackupMod.Infrastructure.Game
{
    public class PlayersProvider : IPlayersProvider
    {
        public PersistentPlayerList GetCurrentPersistentPlayerList()
        {
            return GameManager.Instance.GetPersistentPlayerList();
        }

        public PersistentPlayerData GetPersistentLocalPlayer()
        {
            return GameManager.Instance.GetPersistentLocalPlayer();
        }
    }
}