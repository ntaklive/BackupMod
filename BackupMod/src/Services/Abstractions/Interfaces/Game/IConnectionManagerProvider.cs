namespace BackupMod.Services.Abstractions.Game;

public interface IConnectionManagerProvider
{
    public ConnectionManager GetConnectionManager();
}