using System.IO;
using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class DirectoryService : IDirectoryService
{
    public void Copy(string sourceDirectory, string destinationDirectory, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDirectory);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDirectory);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDirectory, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDirectory, subDir.Name);
                Copy(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return Directory.GetFiles(path, searchPattern, searchOption);
    }

    public string GetParentDirectoryPath(string path)
    {
        return new DirectoryInfo(path).Parent.FullName;
    }

    public string GetDirectoryName(string path)
    {
        return new DirectoryInfo(path).Name;
    }

    public void Delete(string path, bool recursive)
    {
        Directory.Delete(path, recursive);
    }

    public string[] GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }
}