using System;
using BackupMod.Modules.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using static Serilog.Log;

namespace BackupMod.Modules.Serilog;

public sealed class SerilogModule : ModuleBase
{
    public override void InitializeModule(IServiceProvider provider)
    {
        var configuration = ServiceProviderExtensions.GetRequiredService<ModConfiguration>(provider);
        
        string logsDirectoryPath = AssemblyInfo.AssemblyDirectoryPath;

        LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.GameConsole(
                restrictedToMinimumLevel: LogEventLevel.Information
            );

        if (configuration.General.DebugMode)
        {
            loggerConfiguration
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    $"{logsDirectoryPath}/logs/log{Guid.NewGuid().ToString().Replace("-", "")}.txt",
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day
                );
        }

        Logger = loggerConfiguration.CreateLogger();
        
        ModEvents.GameShutdown.RegisterHandler(CloseAndFlush);
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog();
        });
    }
}