using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Application.Commands.Args;

namespace ntaklive.BackupMod.Application.Commands
{
    public interface IConsoleCommand<in T>
        where T : CommandArgs 
    {
        public Result CanExecute(T args);
    
        public Result Execute(T args);
    }
}