#pragma warning disable S107

using System;
using ntaklive.BackupMod.Infrastructure.Game;

namespace ntaklive.BackupMod.Infrastructure
{
    public class GameSavingAlgorithm : IGameSavingAlgorithm
    {
        private readonly Domain.Game _game;
        private readonly IPlayersProvider _playersProvider;
        private readonly IPlayerInputRecordingSystemProvider _recordingSystemProvider;
        private readonly IGamePrefsProvider _gamePrefsProvider;
        private readonly IConnectionManagerProvider _connectionManagerProvider;
        private readonly IThreadManager _threadManager;
        private readonly IPlatformManager _platformManager;
        private readonly IBlock _block;
        private readonly IItem _item;

        public GameSavingAlgorithm(
            Domain.Game game,
            IPlayersProvider playersProvider,
            IPlayerInputRecordingSystemProvider recordingSystemProvider,
            IGamePrefsProvider gamePrefsProvider,
            IConnectionManagerProvider connectionManagerProvider,
            IThreadManager threadManager,
            IPlatformManager platformManager,
            IBlock block,
            IItem item)
        {
            _game = game;
            _playersProvider = playersProvider;
            _recordingSystemProvider = recordingSystemProvider;
            _gamePrefsProvider = gamePrefsProvider;
            _connectionManagerProvider = connectionManagerProvider;
            _threadManager = threadManager;
            _platformManager = platformManager;
            _block = block;
            _item = item;
        }

        public void SaveActiveGame()
        {
            SaveRecordingSystemData();
            SavePrimaryPlayerData();
            SavePersistentPlayersData();
            SaveWorld();
            SaveBlockNameIdMapping();
            SaveItemNameIdMapping();
            SaveGamePrefs();
        }

        private void SaveRecordingSystemData()
        {
            PlayerInputRecordingSystem recordingSystem = _recordingSystemProvider.GetPlayerInputRecordingSystem();
            recordingSystem.AutoSave();
        }

        private void SaveBlockNameIdMapping()
        {
            ConnectionManager connectionManager = _connectionManagerProvider.GetConnectionManager();

            NameIdMapping? blockNameIdMapping = _block.GetNameIdMapping();
            if (blockNameIdMapping != null && connectionManager.IsServer)
            {
                blockNameIdMapping.SaveIfDirty(false);
            }
        }    
    
        private void SaveItemNameIdMapping()
        {
            ConnectionManager connectionManager = _connectionManagerProvider.GetConnectionManager();

            NameIdMapping? itemNameIdMapping = _item.GetNameIdMapping();
            if (itemNameIdMapping != null && connectionManager.IsServer)
            {
                itemNameIdMapping.SaveIfDirty(false);
            }
        }
    
        private void SaveGamePrefs()
        {
            GamePrefs gamePrefs = _gamePrefsProvider.GetGamePrefs();

            gamePrefs.Save();
        }

        private void SaveWorld()
        {
            World world = GameManager.Instance.World;

            world.Save();
            world.SaveDecorations();
            world.SaveWorldState();
        }

        private void SavePrimaryPlayerData()
        {
            World world = GameManager.Instance.World;

            if (_game.ActiveSave! == null!)
            {
                throw new InvalidOperationException("The game is not started yet");
            }
        
            string currentPlayerDataFolderPath = _game.ActiveSave.DirectoryPath;

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

            if (_game.ActiveSave! == null!)
            {
                throw new InvalidOperationException("The game is not started yet");
            }

            string currentWorldSaveFolderPath = _game.ActiveSave.DirectoryPath;
            players.Write(currentWorldSaveFolderPath + "/players.xml");
        }
    }
}