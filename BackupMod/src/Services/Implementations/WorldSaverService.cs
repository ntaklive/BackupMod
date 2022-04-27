#pragma warning disable S107

using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class WorldSaverService : IWorldSaverService
{
    private readonly IWorldService _worldService;
    private readonly IPlayersProvider _playersProvider;
    private readonly IPlayerInputRecordingSystemProvider _recordingSystemProvider;
    private readonly IGamePrefsProvider _gamePrefsProvider;
    private readonly IConnectionManagerProvider _connectionManagerProvider;
    private readonly IThreadManager _threadManager;
    private readonly IPlatformManager _platformManager;
    private readonly IBlock _block;
    private readonly IItem _item;

    public WorldSaverService(
        IWorldService worldService,
        IPlayersProvider playersProvider,
        IPlayerInputRecordingSystemProvider recordingSystemProvider,
        IGamePrefsProvider gamePrefsProvider,
        IConnectionManagerProvider connectionManagerProvider,
        IThreadManager threadManager,
        IPlatformManager platformManager,
        IBlock block,
        IItem item)
    {
        _worldService = worldService;
        _playersProvider = playersProvider;
        _recordingSystemProvider = recordingSystemProvider;
        _gamePrefsProvider = gamePrefsProvider;
        _connectionManagerProvider = connectionManagerProvider;
        _threadManager = threadManager;
        _platformManager = platformManager;
        _block = block;
        _item = item;
    }

    public void SaveAll()
    {
        PlayerInputRecordingSystem recordingSystem = _recordingSystemProvider.GetPlayerInputRecordingSystem();
        recordingSystem.AutoSave();

        SavePrimaryPlayerData();
        SavePersistentPlayersData();
        SaveWorld();

        ConnectionManager connectionManager = _connectionManagerProvider.GetConnectionManager();

        NameIdMapping blockNameIdMapping = _block.GetNameIdMapping();
        if (blockNameIdMapping != null && connectionManager.IsServer)
        {
            blockNameIdMapping.SaveIfDirty(false);
        }

        NameIdMapping itemNameIdMapping = _item.GetNameIdMapping();
        if (itemNameIdMapping != null && connectionManager.IsServer)
        {
            itemNameIdMapping.SaveIfDirty(false);
        }

        SaveGamePrefs();
    }

    private void SaveGamePrefs()
    {
        GamePrefs gamePrefs = _gamePrefsProvider.GetGamePrefs();

        gamePrefs.Save();
    }

    private void SaveWorld()
    {
        World world = _worldService.GetCurrentWorld();

        world.Save();
        world.SaveDecorations();
        world.SaveWorldState();
    }

    private void SavePrimaryPlayerData()
    {
        World world = _worldService.GetCurrentWorld();

        string currentPlayerDataFolderPath = _worldService.GetCurrentWorldSaveDirectory();

        if (world == null)
        {
            return;
        }

        EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
        if (primaryPlayer == null)
        {
            return;
        }

        string combinedString = _platformManager.GetInternalLocalUserIdentifier().CombinedString;

        var playerDataFile = new PlayerDataFile();
        playerDataFile.FromPlayer(primaryPlayer);
        playerDataFile.Save(currentPlayerDataFolderPath, combinedString);

        if (primaryPlayer.ChunkObserver.mapDatabase == null)
        {
            return;
        }

        _threadManager.AddSingleTask(
            primaryPlayer.ChunkObserver.mapDatabase.SaveAsync,
            new MapChunkDatabase.DirectoryPlayerId(currentPlayerDataFolderPath, combinedString));
    }

    private void SavePersistentPlayersData()
    {
        PersistentPlayerList players = _playersProvider.GetCurrentPersistentPlayerList();

        string currentWorldSaveFolderPath = _worldService.GetCurrentWorldSaveDirectory();
        players.Write(currentWorldSaveFolderPath + "/players.xml");
    }
}