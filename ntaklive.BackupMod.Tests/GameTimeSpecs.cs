using System;
using ntaklive.BackupMod.Domain;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class GameTimeSpecs
    {
        [Theory]
        [InlineData(1, 2, 3, "Day: 1, Time: 02:03")]
        [InlineData(1, 12, 30, "Day: 1, Time: 12:30")]
        [InlineData(1, 0, 0, "Day: 1, Time: 00:00")]
        public void Format_GameTimeToValidString_Success(int? day, int? hour, int? minute,
            string expected)
        {
            var gameTime = new GameTime(day, hour, minute);

            Assert.True(gameTime.ToString() == expected);
        }

        [Theory]
        [InlineData(-1, 2, 3)]
        [InlineData(1, -12, 30)]
        [InlineData(1, 0, -1)]
        public void Throw_IfDayOrTimeIsNegative_Success(int? day, int? hour, int? minute)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CreateGameTimeObject(day, hour, minute));
        }

        [Theory]
        [InlineData(1, 61, 3)]
        [InlineData(1, 12, 123)]
        [InlineData(1000, 135, 1234)]
        public void Throw_IfDayOrTimeIsOverflowed_Success(int? day, int? hour, int? minute)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CreateGameTimeObject(day, hour, minute));
        }

        [Theory]
        [InlineData(null, 2, 3)]
        [InlineData(1, null, 30)]
        [InlineData(1, 0, null)]
        public void Throw_IfAnyArgumentIsNull_Success(int? day, int? hour, int? minute)
        {
            Assert.Throws<ArgumentNullException>(() =>
                CreateGameTimeObject(day, hour, minute));
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(1, 12, 30)]
        [InlineData(1, 0, 50)]
        [InlineData(0, 0, 0)]
        public void Create_GameTimeObject_Success(int? day, int? hour, int? minute)
        {
            CreateGameTimeObject(day, hour, minute);

            Assert.True(true);
        }

        [Fact]
        public void Check_IfTwoObjectsAreEqual_True()
        {
            var obj1 = new GameTime(1, 1, 1);
            var obj2 = new GameTime(1, 1, 1);

            Assert.True(obj1 == obj2);
        }

        [Fact]
        public void Check_IfTwoObjectsAreNotEqual_True()
        {
            var obj1 = new GameTime(1, 1, 1);
            var obj2 = new GameTime(3, 4, 5);

            Assert.True(obj1 != obj2);
        }

        private void CreateGameTimeObject(int? day, int? hour, int? minute)
        {
            _ = new GameTime(day, hour, minute);
        }
    }
}