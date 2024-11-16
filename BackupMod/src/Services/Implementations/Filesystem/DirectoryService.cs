using System.Collections.Generic;
using System.IO;
using System.Linq;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Utilities;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services.Filesystem;

public class DirectoryService : IDirectoryService
{
    private readonly ILogger<DirectoryService> _logger;

    public DirectoryService(ILogger<DirectoryService> logger)
    {
        _logger = logger;
    }
    
    public void Copy(string sourcePath, string destinationPath, bool recursive)
    {
        _logger.LogTrace("Copying the {SourcePath} directory to the {DestinationPath} directory", sourcePath, destinationPath);
        
        var directory = new DirectoryInfo(sourcePath);
        if (!directory.Exists)
        {
            throw new DirectoryNotFoundException($"The source directory have not found: {directory.FullName}");
        }

        Create(destinationPath);
        
        foreach (FileInfo file in directory.GetFiles())
        {
            string targetFilepath = Path.Combine(destinationPath, file.Name);
            
            _logger.LogTrace("Copying the file from '{SourcePath}' to '{DestinationPath}' ", targetFilepath, destinationPath);
            file.CopyTo(targetFilepath);
        }

        if (recursive)
        {
            DirectoryInfo[] directories = directory.GetDirectories();
            foreach (DirectoryInfo subDirectory in directories)
            {
                string newDestinationDirectory = Path.Combine(destinationPath, subDirectory.Name);
                Copy(subDirectory.FullName, newDestinationDirectory, true);
            }
        }
    }

    public bool Exists(string path)
    {
        string fixedPath = PathHelper.FixFolderPathSeparators(path);
        
        return Directory.Exists(fixedPath);
    }

    public IReadOnlyCollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return Directory.GetFiles(PathHelper.FixFolderPathSeparators(path), searchPattern, searchOption)
            .Select(PathHelper.FixFolderPathSeparators)
            .ToArray();
    }

    public string GetParentDirectoryPath(string path)
    {
        string fixedPath = PathHelper.FixFolderPathSeparators(path);
        
        return fixedPath[..fixedPath.LastIndexOf(PathHelper.DirectorySeparatorChar)];
    }

    public string GetDirectoryName(string path)
    {
        string fixedPath = PathHelper.FixFolderPathSeparators(path);
        
        return fixedPath.Substring(fixedPath.LastIndexOf(PathHelper.DirectorySeparatorChar) + 1, fixedPath.Length - fixedPath.LastIndexOf(PathHelper.DirectorySeparatorChar) - 1);
    }

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return Directory.EnumerateFiles(path, searchPattern, searchOption);
    }

    public void Delete(string path, bool recursive)
    {
        string fixedPath = PathHelper.FixFolderPathSeparators(path);
        
        Directory.Delete(fixedPath, recursive);
        
        _logger.LogDebug("The {Path} directory has deleted", path);
    }

    public void Create(string path)
    {
        var directory = new DirectoryInfo(path);
        if (!directory.Exists)
        {
            directory.Create();
            
            _logger.LogDebug("The {Path} directory has created", path);
        }
    }

    public IReadOnlyCollection<string> GetDirectories(string path)
    {
        return Directory.GetDirectories(path).Select(PathHelper.FixFolderPathSeparators).ToArray();
    }
}