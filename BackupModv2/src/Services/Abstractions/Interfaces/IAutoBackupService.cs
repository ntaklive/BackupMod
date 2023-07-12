namespace BackupMod.Services.Abstractions;

public interface IAutoBackupService
{
    public bool IsRunning { get; }
    
    public void Start();
    public void Stop();
    public void ResetDelayTimer();
}