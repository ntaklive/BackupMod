using System;
using Serilog.Core;
using Serilog.Events;

namespace BackupMod.Modules.Serilog;

public class GameConsoleSink : ILogEventSink
{
    private const string Template = "[BackupMod]: {0}";
    private const string VerboseTemplate = $"VRB {Template}";
    private const string DebugTemplate = $"DBG {Template}";
    private const string InformationTemplate = $"INF {Template}";
    private const string WarningTemplate = $"WRN {Template}";
    private const string ErrorTemplate = $"ERR {Template}";
    private const string FatalTemplate = $"FTL {Template}";
    
    private readonly IFormatProvider _formatProvider;

    public GameConsoleSink(IFormatProvider formatProvider)
    {
        _formatProvider = formatProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        string message = logEvent.RenderMessage(_formatProvider);
        
        switch (logEvent.Level)
        {
            case LogEventLevel.Verbose: Log.Out(string.Format(VerboseTemplate, message)); break;
            case LogEventLevel.Debug: Log.Out(string.Format(DebugTemplate, message)); break;
            case LogEventLevel.Information: Log.Out(string.Format(InformationTemplate, message)); break;
            case LogEventLevel.Warning: Log.Warning(string.Format(WarningTemplate, message)); break;
            case LogEventLevel.Error: Log.Warning(string.Format(ErrorTemplate, message)); break;
            case LogEventLevel.Fatal: Log.Error(string.Format(FatalTemplate, message)); break;
        }
        
        if (logEvent.Exception != null)
        {
            Log.Exception(logEvent.Exception);
            Log.Error($"Full exception info: {logEvent.Exception}");
        }
    }
}