using Microsoft.Extensions.DependencyInjection;
using NtakliveBackupMod.Services.Abstractions;
using NtakliveBackupMod.Services.Implementations;

namespace NtakliveBackupMod.DI;

public static class LoggerBootstrapper
{
    public static void RegisterLogger(IServiceCollection services)
    {
        services.AddTransient(typeof(ILogger<>),typeof(LogWrapper<>));
    }
}