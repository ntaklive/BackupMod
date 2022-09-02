namespace BackupMod;

public class AutoBackupConfiguration
{
    public AutoBackupConfiguration()
    {
    }
    
    public AutoBackupConfiguration(bool enabled, int delay, bool skipIfThereAreNoPlayers)
    {
        Enabled = enabled;
        Delay = delay;
        SkipIfThereAreNoPlayers = skipIfThereAreNoPlayers;
    }

    public bool Enabled { get; set; }
    public int Delay { get; set; }
    public bool SkipIfThereAreNoPlayers { get; set; }
}