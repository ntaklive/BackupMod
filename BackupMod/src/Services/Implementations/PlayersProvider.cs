using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

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