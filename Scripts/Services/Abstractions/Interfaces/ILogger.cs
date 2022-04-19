using System;

namespace NtakliveBackupMod.Scripts.Services.Abstractions;

public interface ILogger<T>
{
    public void Debug(string message);
    public void Warning(string message);
    public void Error(string message);
    public void Exception(Exception exception);
}