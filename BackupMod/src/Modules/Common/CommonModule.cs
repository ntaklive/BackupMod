using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BackupMod.Modules.Base;
using BackupMod.Services;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Services.Abstractions.Game;
using BackupMod.Services.Filesystem;
using BackupMod.Services.Game;
using BackupMod.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JsonSerializer = BackupMod.Services.JsonSerializer;

namespace BackupMod.Modules.Common;

public sealed partial class CommonModule : ModuleBase
{
    public override void InitializeModule(IServiceProvider provider)
    {
        CreateRequiredFolders(provider);

        ModEvents.GameStartDone.RegisterHandler(() => _ = GameStartDoneHandlerFuncAsync(provider));
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        // JSON
        services.AddSingleton<IJsonSerializer>(_ => new JsonSerializer(
            new JsonSerializerOptions
            {
                WriteIndented = true, Converters = {new JsonStringEnumConverter()}
            }
        ));

        // Filesystem Services
        services.AddSingleton<IArchiveService>(_ => new ZipArchiveService());
        services.AddSingleton<IFileService>(provider => new FileService(
            ServiceProviderExtensions.GetRequiredService<ILogger<FileService>>(provider)
        ));
        services.AddSingleton<IDirectoryService>(provider => new DirectoryService(
            ServiceProviderExtensions.GetRequiredService<ILogger<DirectoryService>>(provider)
        ));
        services.AddSingleton<IPathService>(_ => new PathService());
        services.AddSingleton<IFilesystem>(provider => new Filesystem(
            ServiceProviderExtensions.GetRequiredService<IFileService>(provider),
            ServiceProviderExtensions.GetRequiredService<IDirectoryService>(provider),
            ServiceProviderExtensions.GetRequiredService<IPathService>(provider),
            ServiceProviderExtensions.GetRequiredService<IArchiveService>(provider)
        ));

        // Game Services
        services.AddSingleton<IChatService>(provider =>
        {
            var configuration = ServiceProviderExtensions.GetRequiredService<ModConfiguration>(provider);
            return configuration.Notifications.Enabled
                ? new ChatService(
                    ServiceProviderExtensions.GetRequiredService<IConnectionManagerProvider>(provider),
                    ServiceProviderExtensions.GetRequiredService<ILogger<ChatService>>(provider)
                )
                : null;
        });

        services.AddSingleton<IPlayersProvider>(_ => new PlayersProvider());
        services.AddSingleton<IGamePrefsProvider>(_ => new GamePrefsProvider());
        services.AddSingleton<IPlatformManager>(_ => new PlatformManager());
        services.AddSingleton<IConnectionManagerProvider>(_ => new ConnectionManagerProvider());
        services.AddSingleton<IThreadManager>(_ => new Services.Game.ThreadManager());
        services.AddSingleton<IPlayerInputRecordingSystemProvider>(_ => new PlayerInputRecordingSystemProvider());
        services.AddSingleton<IBlock>(_ => new Services.Game.Block());
        services.AddSingleton<IItem>(_ => new Item());

        // Configuration
        services.AddSingleton<IConfigurationService>(provider =>
        {
            var pathService = ServiceProviderExtensions.GetRequiredService<IPathService>(provider);

            string configFilepath = PathHelper.FixFolderPathSeparators(
                pathService.Combine(pathService.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "settings.json")
            );

            return new ConfigurationService(
                configFilepath,
                ServiceProviderExtensions.GetRequiredService<IFileService>(provider),
                ServiceProviderExtensions.GetRequiredService<IJsonSerializer>(provider),
                ServiceProviderExtensions.GetRequiredService<ILogger<ConfigurationService>>(provider));
        });
        services.AddTransient<ModConfiguration>(provider =>
            ServiceProviderExtensions.GetRequiredService<IConfigurationService>(provider).ReadConfiguration());

        // Services
        services.AddSingleton<IBackupManifestService>(provider => new BackupManifestService(
            ServiceProviderExtensions.GetRequiredService<IFileService>(provider),
            ServiceProviderExtensions.GetRequiredService<IJsonSerializer>(provider)
        ));
        services.AddSingleton<IWorldService>(provider => new WorldService(
            ServiceProviderExtensions.GetRequiredService<IFilesystem>(provider)
        ));
        services.AddTransient<IAutoBackupService>(provider => new AutoBackupService(
            ServiceProviderExtensions.GetRequiredService<ModConfiguration>(provider),
            ServiceProviderExtensions.GetRequiredService<IBackupManager>(provider),
            ServiceProviderExtensions.GetRequiredService<IServerStateWatcher>(provider),
            ServiceProviderExtensions.GetService<IChatService>(provider),
            ServiceProviderExtensions.GetRequiredService<ILogger<AutoBackupService>>(provider)
        ));
        
        services.AddSingleton<IWorldInfoService>(provider => new WorldInfoService(
            ServiceProviderExtensions.GetRequiredService<IFilesystem>(provider),
            ServiceProviderExtensions.GetRequiredService<IResources>(provider),
            ServiceProviderExtensions.GetRequiredService<IBackupManifestService>(provider),
            ServiceProviderExtensions.GetRequiredService<IWorldInfoFactory>(provider)
        ));

        // Factories
        services.AddSingleton<IBackupManifestFactory>(provider => new BackupManifestFactory(
            ServiceProviderExtensions.GetRequiredService<IFilesystem>(provider),
            ServiceProviderExtensions.GetRequiredService<IWorldService>(provider),
            ServiceProviderExtensions.GetRequiredService<IResources>(provider)
        ));
        services.AddSingleton<IBackupInfoFactory>(provider => new BackupInfoFactory(
            ServiceProviderExtensions.GetRequiredService<IFilesystem>(provider),
            ServiceProviderExtensions.GetRequiredService<IResources>(provider),
            ServiceProviderExtensions.GetRequiredService<ILogger<BackupInfoFactory>>(provider)
        ));
        services.AddSingleton<IWorldInfoFactory>(provider => new WorldInfoFactory(
            ServiceProviderExtensions.GetRequiredService<IFilesystem>(provider),
            ServiceProviderExtensions.GetRequiredService<IResources>(provider),
            ServiceProviderExtensions.GetRequiredService<ISaveInfoFactory>(provider),
            ServiceProviderExtensions.GetRequiredService<ILogger<WorldInfoFactory>>(provider)
        ));
        services.AddSingleton<ISaveInfoFactory>(provider => new SaveInfoFactory(
            ServiceProviderExtensions.GetRequiredService<IFilesystem>(provider),
            ServiceProviderExtensions.GetRequiredService<IResources>(provider),
            ServiceProviderExtensions.GetRequiredService<IBackupInfoFactory>(provider),
            ServiceProviderExtensions.GetRequiredService<ILogger<SaveInfoFactory>>(provider)
        ));
        
        // Other
        services.AddTransient<IResources>(provider => new Resources(
            ServiceProviderExtensions.GetRequiredService<ModConfiguration>(provider),
            ServiceProviderExtensions.GetRequiredService<IPathService>(provider),
            ServiceProviderExtensions.GetRequiredService<IDirectoryService>(provider)
        ));
        services.AddSingleton<IWorldSaveAlgorithm>(provider => new WorldSaveAlgorithm(
            ServiceProviderExtensions.GetRequiredService<IWorldService>(provider),
            ServiceProviderExtensions.GetRequiredService<IPlayersProvider>(provider),
            ServiceProviderExtensions.GetRequiredService<IPlayerInputRecordingSystemProvider>(provider),
            ServiceProviderExtensions.GetRequiredService<IGamePrefsProvider>(provider),
            ServiceProviderExtensions.GetRequiredService<IConnectionManagerProvider>(provider),
            ServiceProviderExtensions.GetRequiredService<IThreadManager>(provider),
            ServiceProviderExtensions.GetRequiredService<IPlatformManager>(provider),
            ServiceProviderExtensions.GetRequiredService<IBlock>(provider),
            ServiceProviderExtensions.GetRequiredService<IItem>(provider)
        ));
        services.AddSingleton<IBackupManager>(provider => new BackupManager(
            ServiceProviderExtensions.GetRequiredService<ModConfiguration>(provider),
            ServiceProviderExtensions.GetRequiredService<IBackupManifestService>(provider),
            ServiceProviderExtensions.GetRequiredService<IBackupManifestFactory>(provider),
            ServiceProviderExtensions.GetRequiredService<IWorldInfoService>(provider),
            ServiceProviderExtensions.GetRequiredService<IBackupInfoFactory>(provider),
            ServiceProviderExtensions.GetRequiredService<IResources>(provider),
            ServiceProviderExtensions.GetRequiredService<IWorldService>(provider),
            ServiceProviderExtensions.GetRequiredService<IWorldSaveAlgorithm>(provider),
            ServiceProviderExtensions.GetRequiredService<IFilesystem>(provider),
            ServiceProviderExtensions.GetRequiredService<ILogger<BackupManager>>(provider)
        ));
        services.AddTransient<IServerStateWatcher>(provider => new ServerStateWatcher(
            TimeSpan.FromSeconds(1),
            ServiceProviderExtensions.GetRequiredService<IWorldService>(provider),
            ServiceProviderExtensions.GetRequiredService<ILogger<ServerStateWatcher>>(provider)
        ));
    }

    private static void CreateRequiredFolders(IServiceProvider provider)
    {
        var resources = ServiceProviderExtensions.GetRequiredService<IResources>(provider);
        var directoryService = ServiceProviderExtensions.GetRequiredService<IDirectoryService>(provider);

        directoryService.Create(resources.GetBackupsDirectoryPath());
        directoryService.Create(resources.GetArchiveDirectoryPath());
    }
}