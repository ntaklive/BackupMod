namespace NtakliveBackupMod.Scripts.Services.Abstractions;

public interface IConnectionManagerProvider
{
    public ConnectionManager GetConnectionManager();
}