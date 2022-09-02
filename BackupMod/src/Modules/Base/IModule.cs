using System;
using Microsoft.Extensions.DependencyInjection;

namespace BackupMod.Modules.Base;

internal interface IModule
{
    /// <summary>
    /// Configure services for current modlet
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services);

    /// <summary>
    /// Initialize module in current modlet
    /// </summary>
    /// <param name="provider"></param>
    public void InitializeModule(IServiceProvider provider);
}