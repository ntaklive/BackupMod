using System.IO;
using System.Linq;
using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class DirectoryService : IDirectoryService
{
    public void CopyDirectory(string sourceDirectory, string destinationDirectory, bool recursive)
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
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    public bool IsDirectoryExists(string directory)
    {
        return Directory.Exists(directory);
    }

    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return Directory.GetFiles(path, searchPattern, searchOption)
            .Select(Utils.PathHelper.FixFolderPathSeparators)
            .ToArray();
    }

    public string GetParentDirectoryPath(string path)
    {
        string fixedPath = Utils.PathHelper.FixFolderPathSeparators(path);
        
        return fixedPath.Substring(0, fixedPath.LastIndexOf(Utils.PathHelper.DirectorySeparatorChar));
    }

    public string GetDirectoryName(string path)
    {
        string fixedPath = Utils.PathHelper.FixFolderPathSeparators(path);
        
        return fixedPath.Substring(fixedPath.LastIndexOf(Utils.PathHelper.DirectorySeparatorChar) + 1, fixedPath.Length - fixedPath.LastIndexOf(Utils.PathHelper.DirectorySeparatorChar) - 1);
    }

    public void DeleteDirectory(string path, bool recursive)
    {
        Directory.Delete(path, recursive);
    }

    public void CreateDirectory(string path)
    {
        var directory = new DirectoryInfo(path);
        if (!directory.Exists)
        {
            directory.Create();
        }
    }

    public string[] GetDirectories(string path)
    {
        return Directory.GetDirectories(path).Select(Utils.PathHelper.FixFolderPathSeparators).ToArray();
    }
}