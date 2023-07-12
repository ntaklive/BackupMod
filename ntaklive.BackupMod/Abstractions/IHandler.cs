namespace ntaklive.BackupMod.Abstractions
{
    public interface IHandler<T>
        where T : IDomainEvent
    {
        void Handle(T domainEvent);
    }
}
