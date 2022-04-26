using NtakliveBackupMod.Services.Abstractions;

namespace NtakliveBackupMod.Services.Implementations;

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