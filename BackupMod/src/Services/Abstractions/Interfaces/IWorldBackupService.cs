using System.Threading.Tasks;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IWorldBackupService
{
    public string Backup(SaveInfo saveInfo, BackupMode mode);
    
    public Task<string> BackupAsync(SaveInfo saveInfo, BackupMode mode);
}