using System.Diagnostics.CodeAnalysis;
using BackupMod.Modules.Base;
using Microsoft.Extensions.DependencyInjection;

namespace BackupMod;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ModApi : IModApi
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public void InitMod(Mod _modInstance)
    {
        var services = new ServiceCollection();

        services.AddSingleton<Mod>(_modInstance);
        services.AddModules(new [] {typeof(ModApi)});
        
        this.InitializeModules(services.BuildServiceProvider());
    }
}