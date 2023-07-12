using System;

namespace ntaklive.BackupMod
{
    public static class Filesystem
    {
        public static IFileService File { get; } = new FileService();
    
        public static IDirectoryService Directory { get; } = new DirectoryService();
    
        public static IPathService Path { get; } = new PathService();

        public static IArchiveService Archive(string extension)
        {
            switch (extension)
            {
                case ".zip": return ZipArchive;
                default: throw new ArgumentException("Unsupported archive format");
            }
        }

        private static readonly IArchiveService ZipArchive = new ZipArchiveService();
    }
}