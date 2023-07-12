using System;
using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class BackupTime : ValueObject<BackupTime>
    {
        public BackupTime(GameTime? gameTime, RealDateTime? realDateTime)
        {
            if (gameTime == null!)
            {
                throw new ArgumentNullException(nameof(gameTime));
            }
            if (realDateTime == null!)
            {
                throw new ArgumentNullException(nameof(realDateTime));
            }

            GameTime = gameTime;
            RealTime = realDateTime;
        }

        public static BackupTime Now(GameTime currentGameTime) => new(currentGameTime, RealDateTime.Now);

        public GameTime GameTime { get; }

        public RealDateTime RealTime { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new object[] {GameTime, RealTime};
        }
    }
}