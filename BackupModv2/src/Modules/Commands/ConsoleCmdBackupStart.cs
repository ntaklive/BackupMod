using Microsoft.Extensions.Logging;

namespace BackupMod.Modules.Commands;

public partial class ConsoleCmdBackup
{
    private void BackupStartInternal()
    {
        if (_worldService.GetCurrentWorld() == null)
        {
            _logger.LogError("This command cannot be executed in the main menu");

            return;
        }
        
        // Do nothing ;\
    }
}