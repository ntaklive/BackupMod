using System.IO;
using NtakliveBackupMod.Services.Abstractions;

namespace NtakliveBackupMod.Services.Implementations;

public class FileService : IFileService
{
    public void Delete(string filepath)
    {
        File.Delete(filepath);
    }
}