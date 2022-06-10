using System.Threading.Tasks;

namespace BackupMod.Services.Abstractions;

public interface IFileService
{
    public void Delete(string filepath);
    public void Copy(string sourceFilepath, string destinationFilepath, bool overwrite);

    public void WriteAllText(string filePath, string text);
}