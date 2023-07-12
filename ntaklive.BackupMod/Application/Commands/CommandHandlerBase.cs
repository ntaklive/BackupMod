using System;

namespace ntaklive.BackupMod.Application.Commands
{
    public abstract class CommandHandlerBase : ConsoleCmdAbstract
    {
        protected internal static IServiceProvider? Provider { get; internal set; }
    }
}