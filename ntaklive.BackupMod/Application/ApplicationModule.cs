using System;
using Microsoft.Extensions.DependencyInjection;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Application.Base;
using ntaklive.BackupMod.Domain;
using ntaklive.BackupMod.Infrastructure;

namespace ntaklive.BackupMod.Application
{
   public sealed class ApplicationModule : ModuleBase
   {
      public override void ConfigureServices(IServiceCollection services)
      {
         services.AddSingleton<Game>(provider =>
         {
            Result<Game> createGameResult = provider.GetRequiredService<IGameFactory>().CreateGame();
            if (!createGameResult.IsSuccess)
            {
               throw new InvalidOperationException($"Cannot create a Game instance. Reason: {createGameResult.ErrorMessage}");
            }
            
            return createGameResult.Data!;
         });

         services.AddSingleton<Domain.Mod>(provider => new Domain.Mod(Configuration.Default));
      }

      public override void InitializeModule(IServiceProvider provider)
      {
      }
   }
}