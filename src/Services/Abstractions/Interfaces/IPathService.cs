namespace NtakliveBackupMod.Services.Abstractions;

public interface IPathService
{
    public string GetDirectoryName(string path);
    public string GetFileNameWithoutExtension(string path);
    public string Combine(string arg1, string arg2);
    public string Combine(string arg1, string arg2, string arg3);
    public string Combine(string arg1, string arg2, string arg3, string arg4);
}