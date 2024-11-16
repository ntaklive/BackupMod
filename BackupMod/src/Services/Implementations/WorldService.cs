using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utilities;

namespace BackupMod.Services;

public class WorldService : IWorldService
{
    private readonly IFilesystem _filesystem;

    public WorldService(
        IFilesystem filesystem)
    {
        _filesystem = filesystem;
    }

    public int GetPlayersCount() => GetCurrentWorld().GetPlayers().Count;
    
    public WorldTime GetWorldTime()
    {
        ulong worldTime = GameManager.Instance.World.GetWorldTime();
        int day = GameUtils.WorldTimeToDays(worldTime);
        int hour = GameUtils.WorldTimeToHours(worldTime);
        int minute = GameUtils.WorldTimeToMinutes(worldTime);

        return new WorldTime {Day = day, Hour = hour, Minute = minute, Timestamp = worldTime};
    }
    
    public World GetCurrentWorld() => GameManager.Instance.World;

    public string GetCurrentWorldDirectoryPath() => PathHelper.FixFolderPathSeparators(GameIO.GetWorldDir());

    public string GetCurrentWorldName() => _filesystem.Directory.GetDirectoryName(GetCurrentWorldDirectoryPath());
    
    public string GetCurrentSaveDirectoryPath() => PathHelper.FixFolderPathSeparators(GameIO.GetSaveGameDir());
    
    public string GetCurrentSaveName() => _filesystem.Directory.GetDirectoryName(GetCurrentSaveDirectoryPath());
    
    /// <summary>
    /// Is a local world loaded
    /// </summary>
    public bool IsWorldAccessible()
    {
        World world = GetCurrentWorld();
        return world != null && !world.IsRemote();
    }

    public int GetMaxPlayersCount() => GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount);
    
    public string GetMd5HashForCurrentWorld()
    {
        string checksumsTxtPath = _filesystem.Path.Combine(GetCurrentWorldDirectoryPath(), Constants.cFileWorldChecksums);
        
        return Md5HashHelper.ComputeTextHash(_filesystem.File.ReadAllText(checksumsTxtPath));
    }
}