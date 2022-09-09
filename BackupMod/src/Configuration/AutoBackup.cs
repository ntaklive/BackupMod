namespace BackupMod;

public class AutoBackupConfiguration
{
    public AutoBackupConfiguration()
    {
    }
    
    public AutoBackupConfiguration(bool enabled, int delay, bool skipIfThereAreNoPlayers, bool resetDelayTimerAfterManualBackup)
    {
        Enabled = enabled;
        Delay = delay;
        ResetDelayTimerAfterManualBackup = resetDelayTimerAfterManualBackup;
        SkipIfThereAreNoPlayers = skipIfThereAreNoPlayers;
    }

    public bool Enabled { get; set; }
    public int Delay { get; set; }
    public bool ResetDelayTimerAfterManualBackup { get; set; }
    public bool SkipIfThereAreNoPlayers { get; set; }
}