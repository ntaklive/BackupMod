using System;
using BackupMod.Modules.Base;

namespace BackupMod.Modules.Commands;

public sealed class CommandsModule : ModuleBase
{
   public override void InitializeModule(IServiceProvider provider)
   {
      ConsoleCmdBase.Provider = provider;
   }
}