using System.IO;
using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class FileService : IFileService
{
    public void Delete(string filepath)
    {
        File.Delete(filepath);
    }
}