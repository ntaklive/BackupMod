using System.IO.Compression;
using System.Text;
using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class ArchiveService : IArchiveService
{
    public void CompressFolderToZip(string folderPath, string zipPath, bool includeBaseDirectory = true)
    {
        ZipFile.CreateFromDirectory(folderPath, zipPath, CompressionLevel.Optimal, includeBaseDirectory, Encoding.UTF8);
    }

    public void DecompressZipToFolder(string zipPath, string folderPath)
    {
        ZipFile.ExtractToDirectory(zipPath, folderPath, Encoding.UTF8);
    }
}