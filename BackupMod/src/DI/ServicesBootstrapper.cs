using System.Text.Json;
using System.Text.Json.Serialization;
using BackupMod.Services;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = BackupMod.Services.JsonSerializer;

namespace BackupMod.DI;

public static class ServicesBootstrapper
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IChatService>(provider =>
        {
            var configuration = provider.GetRequiredService<Configuration>();
            return configuration.Utilities.ChatNotificationsEnabled
                ? new ChatService(
                    provider.GetRequiredService<IConnectionManagerProvider>(),
                    provider.GetRequiredService<ILogger<ChatService>>()
                )
                : null;
        });

        services.AddSingleton<IJsonSerializer>(_ => new JsonSerializer(new JsonSerializerOptions()
            {WriteIndented = true, Converters = {new JsonStringEnumConverter()}}
        ));
        
        services.AddSingleton<IArchiveService>(_ => new ArchiveService());
        services.AddSingleton<IFileService>(_ => new FileService());
        services.AddSingleton<IDirectoryService>(_ => new DirectoryService());
        services.AddSingleton<IPathService>(_ => new PathService());
        services.AddSingleton<IPlayersProvider>(_ => new PlayersProvider());
        services.AddSingleton<IGamePrefsProvider>(_ => new GamePrefsProvider());
        services.AddSingleton<IPlatformManager>(_ => new PlatformManager());
        services.AddSingleton<IConnectionManagerProvider>(_ => new ConnectionManagerProvider());
        services.AddSingleton<IThreadManager>(_ => new Services.ThreadManager());
        services.AddSingleton<IPlayerInputRecordingSystemProvider>(_ => new PlayerInputRecordingSystemProvider());
        services.AddSingleton<IBlock>(_ => new Services.Block());
        services.AddSingleton<IItem>(_ => new Item());
        
        services.AddTransient<IGameDirectoriesService>(provider => new GameDirectoriesService(
            provider.GetRequiredService<Configuration>(),
            provider.GetRequiredService<IPathService>(),
            provider.GetRequiredService<IDirectoryService>()
        ));
        services.AddSingleton<IWorldService>(provider => new WorldService(
            provider.GetRequiredService<ISaveInfoFactory>()
        ));
        services.AddSingleton<ISaveInfoFactory>(provider => new SaveInfoFactory(
            provider.GetRequiredService<IDirectoryService>(),
            provider.GetRequiredService<IGameDataProvider>()
        ));
        services.AddSingleton<IGameDataProvider>(provider => new GameDataProvider(
            provider.GetRequiredService<IArchiveService>(),
            provider.GetRequiredService<IPathService>(),
            provider.GetRequiredService<IDirectoryService>(),
            provider.GetRequiredService<IGameDirectoriesService>()
        ));
        services.AddSingleton<IWorldSaverService>(provider => new WorldSaverService(
            provider.GetRequiredService<IWorldService>(),
            provider.GetRequiredService<IPlayersProvider>(),
            provider.GetRequiredService<IPlayerInputRecordingSystemProvider>(),
            provider.GetRequiredService<IGamePrefsProvider>(),
            provider.GetRequiredService<IConnectionManagerProvider>(),
            provider.GetRequiredService<IThreadManager>(),
            provider.GetRequiredService<IPlatformManager>(),
            provider.GetRequiredService<IBlock>(),
            provider.GetRequiredService<IItem>()
        ));
        services.AddTransient<IWorldBackupService>(provider => new WorldBackupService(
            provider.GetRequiredService<Configuration>(),
            provider.GetRequiredService<IGameDirectoriesService>(),
            provider.GetRequiredService<IWorldSaverService>(),
            provider.GetRequiredService<IDirectoryService>(),
            provider.GetRequiredService<IPathService>(),
            provider.GetRequiredService<IArchiveService>(),
            provider.GetRequiredService<IFileService>()
        ));
        services.AddTransient<IBackupWatchdog>(provider => new BackupWatchdog(
            provider.GetRequiredService<Configuration>(),
            provider.GetRequiredService<IWorldService>(),
            provider.GetRequiredService<IWorldBackupService>(),
            provider.GetService<IChatService>(),
            provider.GetRequiredService<ILogger<BackupWatchdog>>()
        ));
    }
}