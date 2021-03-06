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
        ServiceProvider provider = services.BuildServiceProvider();

        var pathService = provider.GetRequiredService<IPathService>();

        string fullPath = Assembly.GetExecutingAssembly().Location;
        string configPath = pathService.Combine(pathService.GetDirectoryName(fullPath)!, "settings.json");

        services.AddSingleton<IConfigurationService>(provider => new ConfigurationService(
            configPath,
            provider.GetRequiredService<IFileService>(),
            provider.GetRequiredService<IJsonSerializer>(),
            provider.GetRequiredService<ILogger<ConfigurationService>>()
        ));

        services.AddTransient<Configuration>(resolver =>
            resolver.GetRequiredService<IConfigurationService>().GetConfiguration());
    }
}