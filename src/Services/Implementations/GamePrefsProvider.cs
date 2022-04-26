using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class GamePrefsProvider : IGamePrefsProvider
{
    public GamePrefs GetGamePrefs()
    {
        return GamePrefs.Instance;
    }
}