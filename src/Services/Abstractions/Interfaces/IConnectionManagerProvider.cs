namespace NtakliveBackupMod.Services.Abstractions;

public interface IConnectionManagerProvider
{
    public ConnectionManager GetConnectionManager();
}