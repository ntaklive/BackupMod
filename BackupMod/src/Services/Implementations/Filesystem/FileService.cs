using System.IO;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Utilities;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services.Filesystem;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }
    
    public void Delete(string filepath)
    {
        string fixedFilepath = PathHelper.FixFolderPathSeparators(filepath);
        
        File.Delete(fixedFilepath);
    }

    public void Copy(string sourceFilepath, string destinationFilepath, bool overwrite)
    {
        string fixedSourceFilepath = PathHelper.FixFolderPathSeparators(sourceFilepath);
        string fixedDestinationFilepath = PathHelper.FixFolderPathSeparators(destinationFilepath);

        File.Copy(fixedSourceFilepath, fixedDestinationFilepath, overwrite);
        
        _logger.LogDebug("");
    }

    public void WriteAllText(string filepath, string text)
    {
        string fixedFilepath = PathHelper.FixFolderPathSeparators(filepath);

        File.WriteAllText(fixedFilepath, text);
    }
    
    public string ReadAllText(string filepath)
    {
        string fixedFilepath = PathHelper.FixFolderPathSeparators(filepath);

        return File.ReadAllText(fixedFilepath);
    }

    public bool Exists(string filepath)
    {
        string fixedFilepath = PathHelper.FixFolderPathSeparators(filepath);

        return File.Exists(fixedFilepath);
    }
}