using System;

namespace ntaklive.BackupMod.Application.Commands.Args
{
    /// <summary>Represents the base class for classes that contain command arguments, and provides a value to use for commands that do not use arguments.</summary>
    [Serializable]
    public class CommandArgs
    {
        /// <summary>Provides a value to use with commands that do not have arguments.</summary>
        public static readonly CommandArgs Empty = new();

        /// <summary>Initializes a new instance of the <see cref="T:ntaklive.BackupMod.Application.Commands.Args.CommandArgs" /> class.</summary>
        public CommandArgs()
        {
        }
    }
}