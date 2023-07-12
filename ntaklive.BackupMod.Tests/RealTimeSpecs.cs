using System;
using ntaklive.BackupMod.Domain;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class RealTimeSpecs
    {
        [Theory]
        [InlineData(-1, 1, 1)]
        [InlineData(1, -1, 1)]
        [InlineData(1, 1, -1)]
        [InlineData(-1, -1, -1)]
        public void Throw_IfTimeIsNegative_Success(
            int? hour, int? minute, int? second)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CreateRealTimeObject(hour, minute, second));
        }

        [Theory]
        [InlineData(25, 1, 1)]
        [InlineData(24, 61, 1)]
        [InlineData(24, 60, 61)]
        [InlineData(25, 61, 61)]
        public void Throw_IfTimeIsOverflowed_Success(
            int? hour, int? minute, int? second)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CreateRealTimeObject(hour, minute, second));
        }
        
        [Theory]
        [InlineData(null, 2, 3)]
        [InlineData(1, null, 30)]
        [InlineData(1, 0, null)]
        public void Throw_IfTimeIsNull_Success(
            int? hour, int? minute, int? second)
        {
            Assert.Throws<ArgumentNullException>(() => 
                CreateRealTimeObject(hour, minute, second));
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(12, 24, 31)]
        [InlineData(23, 59, 59)]
        [InlineData(0, 0, 0)]
        public void Create_RealTimeObject_Success(
            int? hour, int? minute, int? second)
        {
            CreateRealTimeObject(hour, minute, second);

            Assert.True(true);
        }
        
        [Fact]
        public void Check_IfTwoObjectsAreEqual_True()
        {
            var obj1 = new RealTime(1,1,1);
            var obj2 = new RealTime(1,1,1);
            
            Assert.True(obj1 == obj2);
        }
        
        [Fact]
        public void Check_IfTwoObjectsAreNotEqual_True()
        {
            var obj1 = new RealTime(1,1,1);
            var obj2 = new RealTime(3,4,5);
            
            Assert.True(obj1 != obj2);
        }

        private void CreateRealTimeObject(
            int? hour, int? minute, int? second)
        {
            _ = new RealTime(hour, minute, second);
        }
    }
}