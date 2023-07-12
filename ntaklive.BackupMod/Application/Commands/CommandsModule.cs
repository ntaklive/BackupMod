using System;
using Microsoft.Extensions.DependencyInjection;
using ntaklive.BackupMod.Application.Base;

namespace ntaklive.BackupMod.Application.Commands
{
   public sealed class CommandsModule : ModuleBase
   {
      public override void ConfigureServices(IServiceCollection services)
      {
         services.AddSingleton<ICommandParser, CommandParser>();
         services.AddSingleton<BackupCommand>();
         // services.AddSingleton<ICommandParser, CommandParser>();
      }

      public override void InitializeModule(IServiceProvider provider)
      {
         CommandHandlerBase.Provider = provider;
      }
   }
}