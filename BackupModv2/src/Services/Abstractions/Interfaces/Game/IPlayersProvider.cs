namespace BackupMod.Services.Abstractions.Game;

public interface IPlayersProvider
{
    public PersistentPlayerList GetCurrentPersistentPlayerList();
    public PersistentPlayerData GetPersistentLocalPlayer();
}