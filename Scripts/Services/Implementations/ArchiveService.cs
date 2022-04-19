using System.IO.Compression;
using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class ArchiveService : IArchiveService
{
    public void CompressFolderToZip(string folderPath, string zipPath, bool includeBaseDirectory = true)
    {
        ZipFile.CreateFromDirectory(folderPath, zipPath, CompressionLevel.Optimal, includeBaseDirectory);
    }
}