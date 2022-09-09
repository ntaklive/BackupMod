using System.Collections.Generic;
using BackupMod.Modules.Commands.Enums;

namespace BackupMod.Modules.Commands.EventArgs;

public class ConsoleCmdEventArgs : System.EventArgs
{
    public ConsoleCmdEventArgs(ConsoleCmdType commandType, IEnumerable<string> arguments)
    {
        CommandType = commandType;
        Arguments = (IReadOnlyList<string>) arguments;
    }
    
    public ConsoleCmdType CommandType { get; set; }
    public IReadOnlyList<string> Arguments { get; set; }
}