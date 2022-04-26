using NtakliveBackupMod.Services.Abstractions.Models;

namespace NtakliveBackupMod.Services.Abstractions;

public interface IConfigurationProvider
{
    public Configuration GetConfiguration();
}