using Microsoft.Extensions.DependencyInjection;

namespace BackupMod.DI;

public static class Bootstrapper
{
    public static void Initialize()
    {
        var services = new ServiceCollection();
        
        LoggerBootstrapper.RegisterLogger(services);
        ServicesBootstrapper.RegisterServices(services);
        ConfigurationBootstrapper.RegisterConfiguration(services);
        FoldersBootstrapper.CreateRequiredFolders(services);

        ServiceLocator.CreateFrom(services.BuildServiceProvider());
    }
}