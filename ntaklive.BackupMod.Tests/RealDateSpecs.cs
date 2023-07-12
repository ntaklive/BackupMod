using System;
using ntaklive.BackupMod.Domain;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class RealDateSpecs
    {
        [Theory]
        [InlineData(-1, 1, 1)]
        [InlineData(1, -1, 1)]
        [InlineData(1, 1, -1)]
        [InlineData(-1, -1, -1)]
        public void Throw_IfDateIsNegative_Success(
            int? day, int? month, int? year)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CreateRealDateObject(day, month, year));
        }

        [Theory]
        [InlineData(32, 1, 2023)]
        [InlineData(31, 11, 1)]
        [InlineData(25, 13, 2022)]
        [InlineData(32, 13, 2022)]
        public void Throw_IfDateIsOverflowed_Success(
            int? day, int? month, int? year)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CreateRealDateObject(day, month, year));
        }

        [Theory]
        [InlineData(0, 1, 2022)]
        [InlineData(1, 0, 2022)]
        [InlineData(0, 0, 2022)]
        public void Throw_IfDayOrMonthIsZero_Success(
            int? day, int? month, int? year)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CreateRealDateObject(day, month, year));
        }

        [Theory]
        [InlineData(null, 2, 3)]
        [InlineData(1, null, 30)]
        [InlineData(1, 0, null)]
        public void Throw_IfAnyArgumentIsNull_Success(
            int? day, int? month, int? year)
        {
            Assert.Throws<ArgumentNullException>(() => 
                CreateRealDateObject(day, month, year));
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(1, 12, 2022)]
        [InlineData(29, 2, 2024)]
        public void Create_RealDateObject_Success(
            int? day, int? month, int? year)
        {
            CreateRealDateObject(day, month, year);

            Assert.True(true);
        }
        
        [Fact]
        public void Throw_IfLeapYearIsInvalid_Success()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                CreateRealDateObject(29, 2, 2023));
        }
        
        [Fact]
        public void Check_IfTwoObjectsAreEqual_Success()
        {
            var obj1 = new RealDate(1,1,1);
            var obj2 = new RealDate(1,1,1);
            
            Assert.True(obj1 == obj2);
        }
        
        [Fact]
        public void Check_IfTwoObjectsAreNotEqual_Success()
        {
            var obj1 = new RealDate(1,1,1);
            var obj2 = new RealDate(3,4,5);
            
            Assert.True(obj1 != obj2);
        }

        private void CreateRealDateObject(
            int? day, int? month, int? year)
        {
            _ = new RealDate(day, month, year);
        }
    }
}