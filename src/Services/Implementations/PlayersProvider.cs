using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

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