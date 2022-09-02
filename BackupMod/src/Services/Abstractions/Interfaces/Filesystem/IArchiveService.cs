namespace BackupMod.Services.Abstractions.Filesystem;

public interface IArchiveService
{
    public void CompressFolder(string folderPath, string archivePath, bool includeBaseDirectory = true);
    public void DecompressToFolder(string archivePath, string folderPath);
}