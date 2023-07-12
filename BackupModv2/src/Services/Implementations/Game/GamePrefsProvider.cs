using BackupMod.Services.Abstractions.Game;

namespace BackupMod.Services.Game;

public class GamePrefsProvider : IGamePrefsProvider
{
    public GamePrefs GetGamePrefs()
    {
        return GamePrefs.Instance;
    }
}