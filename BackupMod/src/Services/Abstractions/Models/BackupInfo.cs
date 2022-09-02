using System;

namespace BackupMod.Services.Abstractions.Models;

public class BackupInfo
{
    public string Title { get; set; }
    
    public string Filepath { get; init; }
    
    public string ManifestFilepath { get; init; }
    
    public bool Archived { get; init; }
    
    public string SaveDirectoryPath { get; init; }

    public AdditionalInfo Additional { get; set; }

    public class AdditionalInfo
    {
        public int Online { get; set; }
        
        public TimeInfo Time { get; set; }
        
        public class TimeInfo
        {
            public GameTime GameTime { get; set; }
            
            public CreationTime CreationTime { get; set; }
        }
        
        public class CreationTime
        {
            public CreationTime()
            {
            }
    
            public DateTime Timestamp { get; set; }
            public string Formatted { get; set; }
        }

        public class GameTime
        {
            public GameTime()
            {
            }
    
            public int Day { get; set; }
    
            public int Hour { get; set; }
    
            public int Minute { get; set; }

            public override string ToString()
            {
                return $"Day: {Day}, {Hour}:{Minute}";
            }
        }
    }
}