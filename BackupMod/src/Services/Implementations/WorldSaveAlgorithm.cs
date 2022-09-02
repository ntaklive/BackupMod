#pragma warning disable S107

using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Game;

namespace BackupMod.Services;

public class WorldSaveAlgorithm : IWorldSaveAlgorithm
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

    public WorldSaveAlgorithm(
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

    public virtual void Save()
    {
        SaveRecordingSystemData();
        SavePrimaryPlayerData();
        SavePersistentPlayersData();
        SaveWorld();
        SaveBlockNameIdMapping();
        SaveItemNameIdMapping();
        SaveGamePrefs();
    }

    protected virtual void SaveRecordingSystemData()
    {
        PlayerInputRecordingSystem recordingSystem = _recordingSystemProvider.GetPlayerInputRecordingSystem();
        recordingSystem.AutoSave();
    }

    protected virtual void SaveBlockNameIdMapping()
    {
        ConnectionManager connectionManager = _connectionManagerProvider.GetConnectionManager();

        NameIdMapping blockNameIdMapping = _block.GetNameIdMapping();
        if (blockNameIdMapping != null && connectionManager.IsServer)
        {
            blockNameIdMapping.SaveIfDirty(false);
        }
    }    
    
    protected virtual void SaveItemNameIdMapping()
    {
        ConnectionManager connectionManager = _connectionManagerProvider.GetConnectionManager();

        NameIdMapping itemNameIdMapping = _item.GetNameIdMapping();
        if (itemNameIdMapping != null && connectionManager.IsServer)
        {
            itemNameIdMapping.SaveIfDirty(false);
        }
    }
    
    protected virtual void SaveGamePrefs()
    {
        GamePrefs gamePrefs = _gamePrefsProvider.GetGamePrefs();

        gamePrefs.Save();
    }

    protected virtual void SaveWorld()
    {
        World world = _worldService.GetCurrentWorld();

        world.Save();
        world.SaveDecorations();
        world.SaveWorldState();
    }

    protected virtual void SavePrimaryPlayerData()
    {
        World world = _worldService.GetCurrentWorld();

        string currentPlayerDataFolderPath = _worldService.GetCurrentSaveDirectoryPath();

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

    protected virtual void SavePersistentPlayersData()
    {
        PersistentPlayerList players = _playersProvider.GetCurrentPersistentPlayerList();

        string currentWorldSaveFolderPath = _worldService.GetCurrentSaveDirectoryPath();
        players.Write(currentWorldSaveFolderPath + "/players.xml");
    }
}