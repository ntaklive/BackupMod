using System.IO.Compression;
using System.Text;
using BackupMod.Services.Abstractions.Filesystem;

namespace BackupMod.Services.Filesystem;

public class ZipArchiveService : IArchiveService
{
    public string Extension => ".zip";
    
    public void CompressFolder(string folderPath, string archivePath, bool includeBaseDirectory = true)
    {
        ZipFile.CreateFromDirectory(folderPath, archivePath, CompressionLevel.Optimal, includeBaseDirectory, Encoding.UTF8);
    }

    public void DecompressToFolder(string archivePath, string folderPath)
    {
        ZipFile.ExtractToDirectory(archivePath, folderPath, Encoding.UTF8);
    }
}