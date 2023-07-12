using System;
using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class GameTime : ValueObject<GameTime>
    {
        public GameTime(int? day, int? hour, int? minute)
        {
            if (day == null)
            {
                throw new ArgumentNullException(nameof(day));
            }
            if (hour == null)
            {
                throw new ArgumentNullException(nameof(hour));
            }
            if (minute == null)
            {
                throw new ArgumentNullException(nameof(minute));
            }

            if (day < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(day), "Day value cannot be a negative number");
            }
            if (hour < 0 || hour > 24)
            {
                throw new ArgumentOutOfRangeException(nameof(hour), "Hour value cannot be a negative number or greater than 24");
            }
            if (minute < 0 || minute >= 60)
            {
                throw new ArgumentOutOfRangeException(nameof(minute), "Minute value cannot be a negative number or greater than 59");
            }
            
            Day = day.Value;
            Hour = hour.Value;
            Minute = minute.Value;
        }
        
        public int Day { get; }
        
        public int Hour { get; }
        
        public int Minute { get; }
        
        public override string ToString()
        {
            return $"Day: {Day}, Time: {(Hour < 10 ? $"0{Hour}" : $"{Hour}")}:{(Minute < 10 ? $"0{Minute}" : $"{Minute}")}";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new[] {Day.ToString(), Hour.ToString(), Minute.ToString()};
        }
    }
}