using System.Threading.Tasks;

namespace BackupMod.Services.Abstractions;

public interface IBackupWatchdog
{
    public Task StartAsync();
}