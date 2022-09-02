namespace BackupMod;

public sealed class GeneralConfiguration
{
    public GeneralConfiguration()
    {
    }
    
    public GeneralConfiguration(int backupsLimit, string customBackupsFolder, bool debugMode)
    {
        BackupsLimit = backupsLimit;
        CustomBackupsFolder = customBackupsFolder;
        DebugMode = debugMode;
    }


    public int BackupsLimit { get; set; }
    public string CustomBackupsFolder { get; set; }
    public bool DebugMode { get; set; }
}