using System.IO;

namespace BackupMod.Services.Abstractions;

public interface IDirectoryService
{
    public string[] GetDirectories(string path);
    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
    public string GetParentDirectoryPath(string path);
    public string GetDirectoryName(string path);
    public void Delete(string path, bool recursive);
    public void Copy(string sourceDirectory, string destinationDirectory, bool recursive);
}