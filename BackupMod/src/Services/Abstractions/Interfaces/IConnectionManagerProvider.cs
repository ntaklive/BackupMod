namespace BackupMod.Services.Abstractions;

public interface IConnectionManagerProvider
{
    public ConnectionManager GetConnectionManager();
}