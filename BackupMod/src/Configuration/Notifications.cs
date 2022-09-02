namespace BackupMod;

public class NotificationsConfiguration
{
    public NotificationsConfiguration()
    {
    }
    
    public NotificationsConfiguration(bool enabled, CountdownConfiguration countdownConfiguration)
    {
        Enabled = enabled;

        Countdown = countdownConfiguration;
    }

    public bool Enabled { get; set; }

    public CountdownConfiguration Countdown { get; set; }
    
    public class CountdownConfiguration
    {
        public CountdownConfiguration()
        {
        
        }

        public CountdownConfiguration(bool enabled, int countFrom)
        {
            Enabled = enabled;
            CountFrom = countFrom;
        }

        public bool Enabled { get; set; }
        public int CountFrom { get; set; }
    }
}