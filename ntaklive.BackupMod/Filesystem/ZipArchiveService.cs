using System.IO.Compression;
using System.Text;

namespace ntaklive.BackupMod
{
    public class ZipArchiveService : IArchiveService
    {
        public void CompressDirectory(string directoryPath, string archiveFilepath, bool includeBaseDirectory = true)
        {
            ZipFile.CreateFromDirectory(directoryPath, archiveFilepath, CompressionLevel.Optimal, includeBaseDirectory, Encoding.UTF8);
        }

        public void DecompressToDirectory(string archiveFilepath, string directoryPath)
        {
            ZipFile.ExtractToDirectory(archiveFilepath, directoryPath, Encoding.UTF8);
        }
    }
}