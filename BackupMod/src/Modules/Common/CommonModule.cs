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
using BackupMod.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JsonSerializer = BackupMod.Services.JsonSerializer;

namespace BackupMod.Modules.Common;

public sealed partial class CommonModule : ModuleBase
{
    private static IBackupManager _backupManager = null!;
    private static IWorldService _worldService = null!;
    private static IResources _resources = null!;
    private static IDirectoryService _directoryService = null!;
    private static ILogger<CommonModule> _logger = null!;
    
    public override void InitializeModule(IServiceProvider provider)
    {
        _backupManager = ServiceProviderExtensions.GetRequiredService<IBackupManager>(provider);
        _worldService = ServiceProviderExtensions.GetRequiredService<IWorldService>(provider);
        _resources = ServiceProviderExtensions.GetRequiredService<IResources>(provider);
        _directoryService = ServiceProviderExtensions.GetRequiredService<IDirectoryService>(provider);
        _logger = ServiceProviderExtensions.GetRequiredService<ILogger<CommonModule>>(provider);

        CreateRequiredFolders();

        ModEvents.GameStartDone.RegisterHandler((ref ModEvents.SGameStartDoneData _) => GameStartDoneHandlerFuncAsync(provider).ContinueWith(x => {
            if (x.IsFaulted)
            {
                _logger.LogError(x.Exception,"fail.");
            }
        }));
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
            ServiceProviderExtensions.GetRequiredService<IFileService>(provider),
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
                    ServiceProviderExtensions.GetRequiredService<IWorldService>(provider),
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
        services.AddTransient<IAutoBackupProcess>(provider => new AutoBackupProcess(
            ServiceProviderExtensions.GetRequiredService<ModConfiguration>(provider),
            ServiceProviderExtensions.GetRequiredService<IBackupManager>(provider),
            ServiceProviderExtensions.GetRequiredService<IServerStateWatcher>(provider),
            ServiceProviderExtensions.GetService<IWorldSaveAlgorithm>(provider),
            ServiceProviderExtensions.GetService<IChatService>(provider),
            ServiceProviderExtensions.GetRequiredService<ILogger<AutoBackupProcess>>(provider)
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
            ServiceProviderExtensions.GetRequiredService<IFilesystem>(provider)
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
        services.AddTransient<IAutoBackupService>(provider => new AutoBackupService(
            ServiceProviderExtensions.GetRequiredService<IAutoBackupProcess>(provider),
            ServiceProviderExtensions.GetRequiredService<ILogger<AutoBackupService>>(provider)
        ));
    }

    private static void CreateRequiredFolders()
    {
        _directoryService.Create(_resources.GetBackupsDirectoryPath());
        _directoryService.Create(_resources.GetArchiveDirectoryPath());
    }
}