namespace BackupMod.Services.Abstractions;

public interface IArchiveService
{
    public string Extension { get; }
    
    public void CompressFolder(string folderPath, string archivePath, bool includeBaseDirectory = true);
    public void DecompressToFolder(string archivePath, string folderPath);
}