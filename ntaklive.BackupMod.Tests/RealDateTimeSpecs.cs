using System;
using ntaklive.BackupMod.Domain;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class RealDateTimeSpecs
    {
        [Fact]
        public void Throw_IfDateOrTimeIsNull_Success()
        {
            Assert.Throws<ArgumentNullException>(() =>
                CreateRealDateTimeObject(null, null));
        }

        [Fact]
        public void Check_IfTwoObjectsAreEqual_True()
        {
            var obj1 = new RealDateTime(new RealDate(2, 3, 4), new RealTime(2, 3, 4));
            var obj2 = new RealDateTime(new RealDate(2, 3, 4), new RealTime(2, 3, 4));

            Assert.True(obj1 == obj2);
        }

        [Fact]
        public void Check_IfTwoObjectsAreNotEqual_True()
        {
            var obj1 = new RealDateTime(new RealDate(2, 3, 4), new RealTime(2, 3, 4));
            var obj2 = new RealDateTime(new RealDate(1, 1, 1), new RealTime(1, 1, 1));

            Assert.True(obj1 != obj2);
        }

        private void CreateRealDateTimeObject(
            RealDate? realDate, RealTime? realTime)
        {
            _ = new RealDateTime(realDate, realTime);
        }
    }
}