namespace BackupMod.Services.Abstractions.Models;

public struct WorldTime
{
    public ulong Timestamp { get; set; }
    
    public int Day { get; set; }
    
    public int Hour { get; set; }
    
    public int Minute { get; set; }
}