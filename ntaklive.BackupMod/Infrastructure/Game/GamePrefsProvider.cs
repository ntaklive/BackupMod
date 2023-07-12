namespace ntaklive.BackupMod.Infrastructure.Game
{
    public class GamePrefsProvider : IGamePrefsProvider
    {
        public GamePrefs GetGamePrefs()
        {
            return GamePrefs.Instance;
        }
    }
}