using System.Threading;
using System.Threading.Tasks;

namespace BackupMod.Services.Abstractions;

public interface IAutoBackupProcess
{
    public Task StartAsync(CancellationToken token);
    
    public void ResetDelayTimer();
}