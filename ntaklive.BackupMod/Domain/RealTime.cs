using System;
using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class RealTime : ValueObject<RealTime>
    {
        public RealTime(int? hour, int? minute, int? second)
        {
            if (hour == null)
            {
                throw new ArgumentNullException(nameof(hour));
            }

            if (minute == null)
            {
                throw new ArgumentNullException(nameof(minute));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (hour < 0 || hour > 24)
            {
                throw new ArgumentOutOfRangeException(nameof(hour),
                    "Hour value cannot be a negative number or greater than 24");
            }

            if (minute < 0 || minute >= 60)
            {
                throw new ArgumentOutOfRangeException(nameof(minute),
                    "Minute value cannot be a negative number or greater than 59");
            }

            if (second < 0 || second >= 60)
            {
                throw new ArgumentOutOfRangeException(nameof(second),
                    "Second value cannot be a negative number or greater than 59");
            }

            Hour = hour.Value;
            Minute = minute.Value;
            Second = second.Value;
        }

        public static RealTime Now
        {
            get
            {
                DateTime now = DateTime.Now;
                return new RealTime(now.Hour, now.Minute, now.Second);
            }
        }

        public int Hour { get; }

        public int Minute { get; }

        public int Second { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new object[] {Hour.ToString(), Minute.ToString(), Second.ToString()};
        }
    }
}