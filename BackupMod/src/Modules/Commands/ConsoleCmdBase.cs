using System;

namespace BackupMod.Modules.Commands;

public abstract class ConsoleCmdBase : ConsoleCmdAbstract
{
    protected internal static IServiceProvider Provider { get; internal set; }
}