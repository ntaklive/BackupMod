namespace ntaklive.BackupMod.Infrastructure.Game
{
    public interface IConnectionManagerProvider
    {
        public ConnectionManager GetConnectionManager();
    }
}