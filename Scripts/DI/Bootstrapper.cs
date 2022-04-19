using System;
using Microsoft.Extensions.DependencyInjection;
using NtakliveBackupMod.Scripts.Services.Abstractions;
using NtakliveBackupMod.Scripts.Services.Implementations;

namespace NtakliveBackupMod.Scripts.DI;

public static class Bootstrapper
{
    public static IServiceProvider Initialize()
    {
        var services = new ServiceCollection();

        services.AddTransient(typeof(ILogger<>),typeof(LogWrapper<>));

        ServicesBootstrapper.RegisterServices(services);

        return services.BuildServiceProvider();
    }
}