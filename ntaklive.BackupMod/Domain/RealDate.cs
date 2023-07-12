using System;
using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class RealDate : ValueObject<RealDate>
    {
        public RealDate(int? day, int? month, int? year)
        {
            if (day == null)
            {
                throw new ArgumentNullException(nameof(day));
            }

            if (month == null)
            {
                throw new ArgumentNullException(nameof(month));
            }

            if (year == null)
            {
                throw new ArgumentNullException(nameof(year));
            }

            if (year < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Year value cannot be a negative number");
            }

            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month),
                    "Month value cannot be less than zero or greater than 12");
            }

            if (day < 1 || day > DateTime.DaysInMonth(year.Value, month.Value))
            {
                throw new ArgumentOutOfRangeException(nameof(day),
                    "Day value cannot be less than zero or greater than max number of days in the month");
            }

            Day = day.Value;
            Month = month.Value;
            Year = year.Value;
        }

        public static RealDate Now
        {
            get
            {
                DateTime now = DateTime.Now;
                return new RealDate(now.Day, now.Month, now.Year);
            }
        }

        public int Day { get; }

        public int Month { get; }

        public int Year { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new object[] {Day.ToString(), Month.ToString(), Year.ToString()};
        }
    }
}