using System;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Application.Commands.Args;

namespace ntaklive.BackupMod.Application.Commands
{
    public interface ICommandParser
    {
        public ValueResult<(Type commandType, CommandArgs args)> Parse(string args);
    }
}