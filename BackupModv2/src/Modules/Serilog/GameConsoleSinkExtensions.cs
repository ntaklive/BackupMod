using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace BackupMod.Modules.Serilog;

public static class GameConsoleSinkExtensions
{
    public static LoggerConfiguration GameConsole(
        this LoggerSinkConfiguration loggerConfiguration,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
        IFormatProvider formatProvider = null)
    {
        return loggerConfiguration.Sink(new GameConsoleSink(formatProvider), restrictedToMinimumLevel);
    }
}