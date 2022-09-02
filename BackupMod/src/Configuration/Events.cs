namespace BackupMod;

public class EventsConfiguration
{
    public EventsConfiguration()
    {
    }
    
    public EventsConfiguration(bool backupOnWorldLoaded, bool backupOnServerIsEmpty)
    {
        BackupOnWorldLoaded = backupOnWorldLoaded;
        BackupOnServerIsEmpty = backupOnServerIsEmpty;
    }

    public bool BackupOnWorldLoaded { get; set; }
    public bool BackupOnServerIsEmpty { get; set; }
}