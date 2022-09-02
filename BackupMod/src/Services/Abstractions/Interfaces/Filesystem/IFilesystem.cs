namespace BackupMod.Services.Abstractions.Filesystem;

public interface IFilesystem
{
    public IFileService File { get; }
    
    public IDirectoryService Directory { get; }
    
    public IPathService Path { get; }
    
    public IArchiveService Archive { get; }
}