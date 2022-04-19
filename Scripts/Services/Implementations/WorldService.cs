using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class WorldService : IWorldService
{
    public World GetCurrentWorld() => GameManager.Instance.World;
    public string GetCurrentWorldSaveDirectory() => GameIO.GetSaveGameDir();

    public string GetCurrentPlayerDataLocalDirectory() => GameIO.GetPlayerDataLocalDir();

    public string GetCurrentGetPlayerDataDirectory() => GameIO.GetPlayerDataDir();
}