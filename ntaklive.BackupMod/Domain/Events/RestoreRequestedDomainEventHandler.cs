using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain.Events
{
    public class RestoreRequestedDomainEventHandler : IHandler<RestoreRequestedDomainEvent>
    {
        public void Handle(RestoreRequestedDomainEvent domainEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}