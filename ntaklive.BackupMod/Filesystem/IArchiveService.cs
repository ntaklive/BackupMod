namespace ntaklive.BackupMod
{
    public interface IArchiveService
    {
        public void CompressDirectory(string directoryPath, string archiveFilepath, bool includeBaseDirectory = true);
        public void DecompressToDirectory(string archiveFilepath, string directoryPath);
    }
}