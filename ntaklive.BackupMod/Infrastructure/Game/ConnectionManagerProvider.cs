namespace ntaklive.BackupMod.Infrastructure.Game
{
    public class ConnectionManagerProvider : IConnectionManagerProvider
    {
        public ConnectionManager GetConnectionManager()
        {
            return SingletonMonoBehaviour<ConnectionManager>.Instance;
        }
    }
}