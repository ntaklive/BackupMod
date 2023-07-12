using System;
using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class RealDateTime : ValueObject<RealDateTime>
    {
        public RealDateTime(RealDate? realDate, RealTime? realTime)
        {
            if (realDate == null!)
            {
                throw new ArgumentNullException(nameof(realDate));
            }
            if (realTime == null!)
            {
                throw new ArgumentNullException(nameof(realTime));
            }
            
            Date = realDate;
            Time = realTime;
        }

        public static RealDateTime Now => new(RealDate.Now, RealTime.Now);

        public RealDate Date { get; }
        public RealTime Time { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new object[] {Date, Time};
        }
    }
}