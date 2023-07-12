namespace BackupMod;

public class ArchiveConfiguration
{
    public ArchiveConfiguration()
    {
    }
    
    public ArchiveConfiguration(bool enabled, int backupsLimit, string customArchiveFolder)
    {
        Enabled = enabled;
        BackupsLimit = backupsLimit;
        CustomArchiveFolder = customArchiveFolder;
    }

    public bool Enabled { get; set; }
    public int BackupsLimit { get; set; }
    public string CustomArchiveFolder { get; set; }
}