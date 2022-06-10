using System.IO;

namespace BackupMod.Services.Abstractions;

public interface IDirectoryService
{
    public void CreateDirectory(string path);
    public string[] GetDirectories(string path);
    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
    public string GetParentDirectoryPath(string path);
    public string GetDirectoryName(string path);
    public void DeleteDirectory(string path, bool recursive);
    public void CopyDirectory(string sourceDirectory, string destinationDirectory, bool recursive);
    bool IsDirectoryExists(string directory);
}