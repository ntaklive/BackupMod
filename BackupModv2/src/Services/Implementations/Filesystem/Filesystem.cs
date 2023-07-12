using BackupMod.Services.Abstractions.Filesystem;

namespace BackupMod.Services.Filesystem;

public class Filesystem : IFilesystem
{
    public Filesystem(
        IFileService fileService,
        IDirectoryService directoryService,
        IPathService pathService,
        IArchiveService archiveService)
    {
        File = fileService;
        Directory = directoryService;
        Path = pathService;
        Archive = archiveService;
    }
    
    public IFileService File { get; }
    
    public IDirectoryService Directory { get; }
    
    public IPathService Path { get; }
    
    public IArchiveService Archive { get; }
}