using NtakliveBackupMod.Scripts.Services.Abstractions.Models;

namespace NtakliveBackupMod.Scripts.Services.Abstractions;

public interface IConfigurationProvider
{
    public Configuration GetConfiguration();
}