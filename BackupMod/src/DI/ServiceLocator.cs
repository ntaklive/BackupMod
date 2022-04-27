using System;

namespace BackupMod.DI;

public static class ServiceLocator
{
    private static IServiceProvider _resolver;

    public static void CreateFrom(IServiceProvider serviceProvider) => _resolver = serviceProvider;
    
    public static TService GetRequiredService<TService>()
    {
        var service = (TService) _resolver.GetService(typeof(TService));
        if (service is null)
        {
            throw new InvalidOperationException($"Failed to resolve object of type {typeof(TService)}");
        }

        return service;
    }

    public static TService GetService<TService>()
    {
        var service = (TService) _resolver.GetService(typeof(TService));
        
        return service;
    }

}