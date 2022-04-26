using Microsoft.Extensions.DependencyInjection;
using NtakliveBackupMod.Scripts.Services.Abstractions;
using NtakliveBackupMod.Scripts.Services.Implementations;

namespace NtakliveBackupMod.Scripts.DI;

public static class LoggerBootstrapper
{
    public static void RegisterLogger(IServiceCollection services)
    {
        services.AddTransient(typeof(ILogger<>),typeof(LogWrapper<>));
    }
}