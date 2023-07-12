using System;
using BackupMod.Modules.Base;
using Microsoft.Extensions.DependencyInjection;

namespace BackupMod.Modules.Commands;

public sealed class CommandsModule : ModuleBase
{
   public override void ConfigureServices(IServiceCollection services)
   {
   }

   public override void InitializeModule(IServiceProvider provider)
   {
      ConsoleCmdBase.Provider = provider;
   }
}