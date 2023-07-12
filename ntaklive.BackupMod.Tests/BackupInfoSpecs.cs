using System;
using ntaklive.BackupMod.Domain;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class BackupInfoSpecs
    {
        private static readonly GameTime TenOClockOfFirstDay = new(1, 10, 0);
        private static readonly GameTime SixOClockOfSecondDay = new(2, 6, 0);

        private static readonly RealDate Epoch = new(1, 1, 1970);
    
        private static readonly RealTime Midnight = new(0, 0, 0);
        private static readonly RealTime Noon = new(12, 0, 0);
    
        [Theory]
        [InlineData(null, null, null)]
        public void Throw_IfAnyArgumentIsNull_Success(
            string? title, BackupCaller? backupType, BackupTime? backupTime)
        {
            Assert.Throws<ArgumentNullException>(() => 
                CreateBackupInfoObject(title, backupType, backupTime));
        }
    
        [Fact]
        public void Check_IfTwoObjectsAreEqual_True()
        {
            var obj1 = new BackupInfo(string.Empty, BackupCaller.Command, new BackupTime(TenOClockOfFirstDay, new RealDateTime(Epoch, Midnight)));
            var obj2 = new BackupInfo(string.Empty, BackupCaller.Command, new BackupTime(TenOClockOfFirstDay, new RealDateTime(Epoch, Midnight)));
            
            Assert.True(obj1 == obj2);
        }
        
        [Fact]
        public void Check_IfTwoObjectsAreNotEqual_True()
        {
            var obj1 = new BackupInfo(string.Empty, BackupCaller.Command, new BackupTime(TenOClockOfFirstDay, new RealDateTime(Epoch, Midnight)));
            var obj2 = new BackupInfo(string.Empty, BackupCaller.Event, new BackupTime(SixOClockOfSecondDay, new RealDateTime(Epoch, Noon)));
        
            Assert.True(obj1 != obj2);
        }
    
        private void CreateBackupInfoObject(
            string? title, BackupCaller? backupType, BackupTime? backupTime)
        {
            _ = new BackupInfo(title, backupType, backupTime);
        }
    }
}