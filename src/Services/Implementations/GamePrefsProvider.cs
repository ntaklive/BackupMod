using NtakliveBackupMod.Services.Abstractions;

namespace NtakliveBackupMod.Services.Implementations;

public class GamePrefsProvider : IGamePrefsProvider
{
    public GamePrefs GetGamePrefs()
    {
        return GamePrefs.Instance;
    }
}