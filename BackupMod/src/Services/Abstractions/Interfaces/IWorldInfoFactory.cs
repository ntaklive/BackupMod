using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IWorldInfoFactory
{
    public WorldInfo CreateFromWorldFolderPath(string worldFolderPath);
}