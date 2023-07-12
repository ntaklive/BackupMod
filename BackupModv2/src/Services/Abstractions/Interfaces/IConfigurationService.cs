namespace BackupMod.Services.Abstractions;

public interface IConfigurationService
{
    public ModConfiguration ReadConfiguration();

    public bool TryUpdateConfiguration(ModConfiguration configuration);
}