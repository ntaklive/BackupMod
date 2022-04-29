namespace BackupMod.Services.Abstractions;

public interface IArchiveService
{
    public void CompressFolderToZip(string folderPath, string zipPath, bool includeBaseDirectory = true);
    public void DecompressZipToFolder(string zipPath, string folderPath);
}