namespace NtakliveBackupMod.Scripts.Services.Abstractions;

public interface IArchiveService
{
    public void CompressFolderToZip(string folderPath, string zipPath, bool includeBaseDirectory = true);
}