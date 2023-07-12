using System.Collections.Generic;
using System.IO;

namespace ntaklive.BackupMod
{
    public interface IDirectoryService
    {
        public void Create(string path);
    
        public void Delete(string path, bool recursive);
    
        public void Copy(string sourcePath, string destinationPath, bool recursive);
    
        bool Exists(string path);
    
        public IReadOnlyCollection<string> GetDirectories(string path);
    
        public IReadOnlyCollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption);
    
        public string GetParentDirectoryPath(string path);
    
        public string GetDirectoryName(string path);
    
        IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
    }
}