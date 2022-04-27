using BackupMod.Services;
using BackupMod.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BackupMod.DI;

public static class LoggerBootstrapper
{
    public static void RegisterLogger(IServiceCollection services)
    {
        services.AddTransient(typeof(ILogger<>),typeof(LogWrapper<>));
    }
}