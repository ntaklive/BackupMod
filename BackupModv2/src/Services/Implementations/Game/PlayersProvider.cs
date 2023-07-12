using BackupMod.Services.Abstractions.Game;

namespace BackupMod.Services.Game;

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