using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace ntaklive.BackupMod.Application.Base
{
    public static class ModuleExtensions
    {
        public static void AddModules(this IServiceCollection services, IEnumerable<Type> entryPointsAssembly)
        {
            var modules = new List<IModule>();
            foreach (Type entryPoint in entryPointsAssembly)
            {
                IEnumerable<Type> types = entryPoint.Assembly.ExportedTypes.Where(x => !x.IsAbstract && typeof(IModule).IsAssignableFrom(x));
                IEnumerable<IModule> instances = types.Select(Activator.CreateInstance).Cast<IModule>();
                modules.AddRange(instances);
            }

            modules.ForEach(module => module.ConfigureServices(services));

            services.AddSingleton<IReadOnlyCollection<IModule>>(modules as IReadOnlyCollection<IModule>);
        }

        public static void InitializeModules(this IModApi _, IServiceProvider provider)
        {
            var modules = provider.GetRequiredService<IReadOnlyCollection<IModule>>();

            modules.ForEach(module => module.InitializeModule(provider));
        }
    }
}