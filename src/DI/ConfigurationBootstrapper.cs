using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NtakliveBackupMod.Services.Abstractions;
using NtakliveBackupMod.Services.Abstractions.Models;
using NtakliveBackupMod.Services.Implementations;

namespace NtakliveBackupMod.DI;

public static class ConfigurationBootstrapper
{
    public static void RegisterConfiguration(IServiceCollection services)
    {
        ServiceProvider resolver = services.BuildServiceProvider();

        var pathService = resolver.GetRequiredService<IPathService>();
        
        string fullPath = Assembly.GetExecutingAssembly().Location;
        string configPath = pathService.Combine(pathService.GetDirectoryName(fullPath)!, "settings.json");

        services.AddSingleton<IConfigurationProvider>(_ => new ConfigurationProvider(configPath));
        services.AddTransient<Configuration>(resolver =>
            resolver.GetRequiredService<IConfigurationProvider>().GetConfiguration());
    }
}