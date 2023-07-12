using System;

namespace ntaklive.BackupMod.Application.Commands.Args
{
    public class BackupCommandArgs : CommandArgs
    {
        public BackupCommandArgs(string? title)
        {
            if (title == null!)
            {
                throw new ArgumentNullException(nameof(title));
            }
        
            Title = title;
        }

        public string Title { get; }
    }
}