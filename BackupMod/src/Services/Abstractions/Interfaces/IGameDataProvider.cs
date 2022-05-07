using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IGameDataProvider
{
    public WorldInfo[] GetWorldsData();
}