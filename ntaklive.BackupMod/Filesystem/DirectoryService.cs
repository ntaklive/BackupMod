using System.Collections.Generic;
using System.IO;
using System.Linq;
using ntaklive.BackupMod.Utils;

namespace ntaklive.BackupMod
{
    public class DirectoryService : IDirectoryService
    {
        public void Copy(string sourcePath, string destinationPath, bool recursive)
        {
            var directory = new DirectoryInfo(sourcePath);
            if (!directory.Exists)
            {
                throw new DirectoryNotFoundException($"The source directory have not found: {directory.FullName}");
            }

            Create(destinationPath);
        
            foreach (FileInfo file in directory.GetFiles())
            {
                string targetFilepath = Path.Combine(destinationPath, file.Name);
            
                file.CopyTo(targetFilepath);
            }

            if (recursive)
            {
                DirectoryInfo[] directories = directory.GetDirectories();
                foreach (DirectoryInfo subDirectory in directories)
                {
                    string newDestinationDirectory = Path.Combine(destinationPath, subDirectory.Name);
                    Copy(subDirectory.FullName, newDestinationDirectory, true);
                }
            }
        }

        public bool Exists(string path)
        {
            string fixedPath = PathHelper.FixFolderPathSeparators(path);
        
            return Directory.Exists(fixedPath);
        }

        public IReadOnlyCollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(PathHelper.FixFolderPathSeparators(path), searchPattern, searchOption)
                .Select(PathHelper.FixFolderPathSeparators)
                .ToArray();
        }

        public string GetParentDirectoryPath(string path)
        {
            string fixedPath = PathHelper.FixFolderPathSeparators(path);
        
            return fixedPath.Substring(0, fixedPath.LastIndexOf(PathHelper.DirectorySeparatorChar));
        }

        public string GetDirectoryName(string path)
        {
            string fixedPath = PathHelper.FixFolderPathSeparators(path);
        
            return fixedPath.Substring(fixedPath.LastIndexOf(PathHelper.DirectorySeparatorChar) + 1, fixedPath.Length - fixedPath.LastIndexOf(PathHelper.DirectorySeparatorChar) - 1);
        }

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(path, searchPattern, searchOption);
        }

        public void Delete(string path, bool recursive)
        {
            string fixedPath = PathHelper.FixFolderPathSeparators(path);
        
            Directory.Delete(fixedPath, recursive);
        }

        public void Create(string path)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        public IReadOnlyCollection<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path).Select(PathHelper.FixFolderPathSeparators).ToArray();
        }
    }
}