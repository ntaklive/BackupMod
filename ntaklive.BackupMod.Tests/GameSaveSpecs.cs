using System;
using ntaklive.BackupMod.Domain;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class GameSaveSpecs
    {
        [Fact]
        public void Throw_IfAnyArgumentIsNull_Success()
        {
            Assert.Throws<ArgumentNullException>(() =>
                CreateGameSaveObject(null, null, null));
        }
    
        private void CreateGameSaveObject(
            string? name, string? directoryPath, GameWorld? world)
        {
            _ = new GameSave(name, directoryPath, world);
        }
    }
}