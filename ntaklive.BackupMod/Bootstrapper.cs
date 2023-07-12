using Microsoft.Extensions.DependencyInjection;
using ntaklive.BackupMod.Application.Base;

namespace ntaklive.BackupMod
{
    public class Bootstrapper : IModApi
    {
        // ReSharper disable once InconsistentNaming
        public void InitMod(Mod _modInstance)
        {
            var services = new ServiceCollection();

            services.AddSingleton<Mod>(_modInstance);
            services.AddModules(new[] {typeof(Bootstrapper)});

            this.InitializeModules(services.BuildServiceProvider());
        }
    }
}