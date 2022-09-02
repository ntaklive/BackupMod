using System;
using Microsoft.Extensions.DependencyInjection;

namespace BackupMod.Modules.Base;

/// <summary>
/// Base implementation for <see cref="IModule"/>
/// </summary>
public abstract class ModuleBase : IModule
{
    public virtual void ConfigureServices(IServiceCollection services)
    {
    }
    
    public virtual void InitializeModule(IServiceProvider provider)
    {
    }
}