using System.IO;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class FileService : IFileService
{
    public void Delete(string filepath)
    {
        File.Delete(filepath);
    }

    public void Copy(string sourceFilepath, string destinationFilepath, bool overwrite)
    {
        File.Copy(sourceFilepath, destinationFilepath, overwrite);
    }

    public void WriteAllText(string filePath, string text)
    {
        File.WriteAllText(filePath, text);
    }
}