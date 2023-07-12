using System;
using BackupMod.Modules.Commands.EventArgs;

namespace BackupMod.Modules.Commands;

public abstract class ConsoleCmdBase : ConsoleCmdAbstract
{
    public static event EventHandler<ConsoleCmdEventArgs> CommandExecuted;
    protected internal static IServiceProvider Provider { get; internal set; }

    protected static void OnCommandExecuted(ConsoleCmdEventArgs e)
    {
        CommandExecuted?.Invoke(null, e);
    }
}