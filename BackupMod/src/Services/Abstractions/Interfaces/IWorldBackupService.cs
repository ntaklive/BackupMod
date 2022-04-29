using System.Threading.Tasks;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IWorldBackupService
{
    public string Backup(SaveInfo saveInfo, BackupMode mode);
    
    public Task<string> BackupAsync(SaveInfo saveInfo, BackupMode mode);

    public void Restore(SaveInfo saveInfo, BackupInfo backupInfo);
    
    public Task RestoreAsync(SaveInfo saveInfo, BackupInfo backupInfo);

    public BackupInfo[] GetAllBackups(SaveInfo saveInfo);

    public void DeleteAllTempFolders(SaveInfo saveInfo);

    public string GetBackupsFolderPath(SaveInfo saveInfo);
    
    public string GetAllBackupsFolderPath(SaveInfo saveInfo);

    public void DeleteRedundantBackupFiles(SaveInfo saveInfo);
}