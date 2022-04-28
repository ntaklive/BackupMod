using System.Reflection;
using BackupMod.Services;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BackupMod.DI;

public static class ConfigurationBootstrapper
{
    public static void RegisterConfiguration(IServiceCollection services)
    {
        ServiceProvider resolver = services.BuildServiceProvider();

        var pathService = resolver.GetRequiredService<IPathService>();

        string fullPath = Assembly.GetExecutingAssembly().Location;
        string configPath = pathService.Combine(pathService.GetDirectoryName(fullPath)!, "settings.json");

        services.AddSingleton<IConfigurationProvider>(resolver => new ConfigurationProvider(
            configPath,
            resolver.GetRequiredService<ILogger<ConfigurationProvider>>()
        ));
        services.AddTransient<Configuration>(resolver =>
            resolver.GetRequiredService<IConfigurationProvider>().GetConfiguration());
    }
}