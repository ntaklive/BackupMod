using System;
using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class LogWrapper<T> : ILogger<T>
{
    private const string Template = "[BackupMod]: {0}";

    public void Debug(string message)
    {
        Log.Out(string.Format(Template, message));
    }

    public void Warning(string message)
    {
        Log.Warning(string.Format(Template, message));
    }

    public void Error(string message)
    {
        Log.Error(string.Format(Template, message));
    }

    public void Exception(Exception exception)
    {
        Log.Exception(exception);
        
        Log.Error("Full error message:");
        Log.Error(exception.ToString());
    }
}