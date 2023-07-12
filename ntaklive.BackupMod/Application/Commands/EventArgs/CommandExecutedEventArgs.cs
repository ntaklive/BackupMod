using System;

namespace ntaklive.BackupMod.Application.Commands.EventArgs
{
    public class CommandExecutedEventArgs : System.EventArgs
    {
        public Type CommandType { get; }

        public CommandExecutedEventArgs(Type commandType)
        {
            CommandType = commandType;
        }
    }
}