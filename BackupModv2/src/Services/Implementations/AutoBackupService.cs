using System.Threading;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services;

public class AutoBackupService : IAutoBackupService
{
    private readonly IAutoBackupProcess _autoBackupProcess;
    private readonly ILogger<AutoBackupService> _logger;
    
    private CancellationTokenSource _cts;

    public AutoBackupService(
        IAutoBackupProcess autoBackupProcess,
        ILogger<AutoBackupService> logger)
    {
        _autoBackupProcess = autoBackupProcess;
        _logger = logger;
        
        _cts = new CancellationTokenSource();
    }
    
    public void Start()
    {
        Task task = _autoBackupProcess.StartAsync(_cts.Token);
        task.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                _logger.LogError(task.Exception, "An unexpected error was occured");
            }

            IsRunning = false;
        });
        
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
        
        _cts.Cancel();
        _cts = new CancellationTokenSource();
    }

    public void ResetDelayTimer()
    {
        _autoBackupProcess.ResetDelayTimer();
    }

    public bool IsRunning { get; private set; }
}