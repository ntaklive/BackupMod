namespace BackupMod.Manifest;

public class AdditionalInfoManifestPart
{
    public AdditionalInfoManifestPart()
    {
    }
    
    public int Online { get; set; }
    
    public GameTimeManifestPart GameTime { get; set; }
    
    public class GameTimeManifestPart
    {
        public GameTimeManifestPart()
        {
        }
        
        public string Timestamp { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}