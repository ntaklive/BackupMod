using System;
using ntaklive.BackupMod.Domain;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class BackupTimeSpecs
    {
        [Fact]
        public void Throw_IfAnyArgumentIsNull_Success()
        {
            Assert.Throws<ArgumentNullException>(() =>
                CreateBackupTimeObject(null, null));
        }
    
        [Fact]
        public void Check_IfTwoObjectsAreEqual_True()
        {
            var obj1 = new BackupTime(new GameTime(29, 2, 59), new RealDateTime(new RealDate(29, 2, 2024), new RealTime(0, 0, 0)));
            var obj2 = new BackupTime(new GameTime(29, 2, 59), new RealDateTime(new RealDate(29, 2, 2024), new RealTime(0, 0, 0)));

            Assert.True(obj1 == obj2);
        }

        [Fact]
        public void Check_IfTwoObjectsAreNotEqual_True()
        {
            var obj1 = new BackupTime(new GameTime(29, 2, 59), new RealDateTime(new RealDate(29, 2, 2024), new RealTime(0, 0, 0)));
            var obj2 = new BackupTime(new GameTime(1, 0, 0), new RealDateTime(new RealDate(28, 2, 2024), new RealTime(1, 2, 3)));

            Assert.True(obj1 != obj2);
        }

        private void CreateBackupTimeObject(
            GameTime? gameTime, RealDateTime? realDateTime)
        {
            _ = new BackupTime(gameTime, realDateTime);
        }
    }
}