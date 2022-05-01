using System.Collections.Generic;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface ISavesProvider
{
    public IEnumerable<WorldInfo> GetAllWorlds();
}