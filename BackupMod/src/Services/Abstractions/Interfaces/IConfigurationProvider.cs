using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IConfigurationProvider
{
    public Configuration GetConfiguration();
}