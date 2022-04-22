using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NtakliveBackupMod.Scripts.Services.Abstractions;
using NtakliveBackupMod.Scripts.Services.Abstractions.Models;
using NtakliveBackupMod.Scripts.Services.Implementations;

namespace NtakliveBackupMod.Scripts.DI;

public static class ServicesBootstrapper
{
    public static void RegisterServices(IServiceCollection services)
    {
        string fullPath = Assembly.GetExecutingAssembly().Location;
        string configPath = Path.Combine(Path.GetDirectoryName(fullPath)!, "settings.json");

        services.AddSingleton<IConfigurationProvider>(_ => new ConfigurationProvider(configPath));
        services.AddTransient<Configuration>(resolver => resolver.GetRequiredService<IConfigurationProvider>().GetConfiguration());

        services.AddSingleton<IArchiveService>(_ => new ArchiveService());
        services.AddSingleton<IFileService>(_ => new FileService());
        services.AddSingleton<IDirectoryService>(_ => new DirectoryService());
        services.AddSingleton<IWorldService>(_ => new WorldService());
        services.AddSingleton<IPlayersProvider>(_ => new PlayersProvider());
        services.AddSingleton<IGamePrefsProvider>(_ => new GamePrefsProvider());
        services.AddSingleton<IPlatformManager>(_ => new PlatformManager());
        services.AddSingleton<IPathService>(_ => new PathService());
        services.AddSingleton<IChatService>(_ => new ChatService());
        services.AddSingleton<IConnectionManagerProvider>(_ => new ConnectionManagerProvider());
        services.AddSingleton<IThreadManager>(_ => new Services.Implementations.ThreadManager());
        services.AddSingleton<IPlayerInputRecordingSystemProvider>(_ => new PlayerInputRecordingSystemProvider());
        services.AddSingleton<IBlock>(_ => new Services.Implementations.Block());
        services.AddSingleton<IItem>(_ => new Item());
        services.AddSingleton<IWorldSaverService>(resolver => new WorldSaverService(
            resolver.GetRequiredService<IWorldService>(),
            resolver.GetRequiredService<IPlayersProvider>(),
            resolver.GetRequiredService<IPlayerInputRecordingSystemProvider>(),
            resolver.GetRequiredService<IGamePrefsProvider>(),
            resolver.GetRequiredService<IConnectionManagerProvider>(),
            resolver.GetRequiredService<IThreadManager>(),
            resolver.GetRequiredService<IPlatformManager>(),
            resolver.GetRequiredService<IBlock>(),
            resolver.GetRequiredService<IItem>()
        ));
        services.AddSingleton<IWorldBackupService>(resolver => new WorldBackupService(
            resolver.GetRequiredService<Configuration>(),
            resolver.GetRequiredService<IWorldSaverService>(),
            resolver.GetRequiredService<IDirectoryService>(),
            resolver.GetRequiredService<IPathService>(),
            resolver.GetRequiredService<IArchiveService>(),
            resolver.GetRequiredService<IFileService>()
        ));
        services.AddSingleton<IBackupWatchdog>(resolver => new BackupWatchdog(
            resolver.GetRequiredService<Configuration>(),
            resolver.GetRequiredService<IWorldBackupService>(),
            resolver.GetRequiredService<IChatService>(),
            resolver.GetRequiredService<ILogger<BackupWatchdog>>()
        ));
    }
}