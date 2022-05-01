using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface ISaveInfoFactory
{
    public SaveInfo GetFromSaveFolderPath(string saveFolderPath);
}