using System;

namespace NtakliveBackupMod.Extensions;

public static class ServicesProviderExtensions
{
    public static TService GetRequiredService<TService>(this IServiceProvider resolver)
    {
        var service = (TService) resolver.GetService(typeof(TService));
        if (service is null)
        {
            throw new InvalidOperationException($"Failed to resolve object of type {typeof(TService)}");
        }

        return service;
    }
}