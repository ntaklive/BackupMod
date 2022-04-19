using System.IO;
using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class FileService : IFileService
{
    public void Delete(string filepath)
    {
        File.Delete(filepath);
    }
}