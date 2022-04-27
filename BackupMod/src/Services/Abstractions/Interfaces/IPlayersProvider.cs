namespace BackupMod.Services.Abstractions;

public interface IPlayersProvider
{
    public PersistentPlayerList GetCurrentPersistentPlayerList();
    public PersistentPlayerData GetPersistentLocalPlayer();
}