using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class GamePrefsProvider : IGamePrefsProvider
{
    public GamePrefs GetGamePrefs()
    {
        return GamePrefs.Instance;
    }
}