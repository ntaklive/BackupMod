using ntaklive.BackupMod.Infrastructure;
using Xunit;

namespace ntaklive.BackupMod.Tests;

public class GameFactoryTests
{
    [Fact]
    public void Create_Game_Success()
    {
        IBackupService backupService = new BackupService();
        IGameSaveFactory saveFactory = new GameSaveFactory();
        IGameWorldFactory worldFactory = new GameWorldFactory(saveFactory, null);
        IGameFactory gameFactory = new GameFactory(worldFactory);
        
        var game 
    }
}