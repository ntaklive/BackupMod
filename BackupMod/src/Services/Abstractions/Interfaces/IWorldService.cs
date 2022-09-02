using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IWorldService
{
    public int GetPlayersCount();
    
    public WorldTime GetWorldTime();
    
    public World GetCurrentWorld();

    public string GetCurrentWorldDirectoryPath();
    
    public string GetCurrentWorldName();
    
    public string GetCurrentSaveDirectoryPath();
    
    public string GetCurrentSaveName();
    
    public bool IsWorldAccessible();

    public int GetMaxPlayersCount();
}