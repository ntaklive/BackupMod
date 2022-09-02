using System;
using Serilog.Core;
using Serilog.Events;

namespace BackupMod.Modules.Serilog;

public class GameConsoleSink : ILogEventSink
{
    private const string Template = "[BackupMod]: {0}";
    private readonly IFormatProvider _formatProvider;

    public GameConsoleSink(IFormatProvider formatProvider)
    {
        _formatProvider = formatProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        string message = logEvent.RenderMessage(_formatProvider);
        string formattedMessage = string.Format(Template, message);
        
        switch (logEvent.Level)
        {
            case LogEventLevel.Verbose: break;
            case LogEventLevel.Debug: break;
            case LogEventLevel.Information: Log.Out(formattedMessage); break;
            case LogEventLevel.Warning: Log.Warning(formattedMessage); break;
            case LogEventLevel.Error: Log.Error(formattedMessage); break;
        }
        
        if (logEvent.Exception != null)
        {
            Log.Exception(logEvent.Exception);
            Log.Error($"Full exception info: {logEvent.Exception}");
        }
    }
}