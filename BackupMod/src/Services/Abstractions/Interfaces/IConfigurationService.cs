namespace BackupMod.Services.Abstractions;

public interface IConfigurationService
{
    public Configuration GetConfiguration();

    public bool TryUpdateConfiguration(Configuration configuration);
}