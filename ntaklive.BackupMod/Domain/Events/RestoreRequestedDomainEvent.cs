using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain.Events
{
    public class RestoreRequestedDomainEvent : IDomainEvent
    {
        public Backup Backup { get; }

        public RestoreRequestedDomainEvent(Backup backup)
        {
            Backup = backup;
        }
    }
}