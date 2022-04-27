using System.IO.Compression;
using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class ArchiveService : IArchiveService
{
    public void CompressFolderToZip(string folderPath, string zipPath, bool includeBaseDirectory = true)
    {
        ZipFile.CreateFromDirectory(folderPath, zipPath, CompressionLevel.Optimal, includeBaseDirectory);
    }
}