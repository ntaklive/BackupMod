using System;
using Microsoft.Extensions.DependencyInjection;

namespace ntaklive.BackupMod.Application.Base
{
    /// <summary>
    /// Base implementation for <see cref="IModule"/>
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        public abstract void ConfigureServices(IServiceCollection services);

        public abstract void InitializeModule(IServiceProvider provider);
    }
}