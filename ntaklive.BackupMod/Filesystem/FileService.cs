using System.IO;
using ntaklive.BackupMod.Utils;

namespace ntaklive.BackupMod
{
    public class FileService : IFileService
    {
        public void Delete(string filepath)
        {
            string fixedFilepath = PathHelper.FixFolderPathSeparators(filepath);
        
            File.Delete(fixedFilepath);
        }

        public void Copy(string sourceFilepath, string destinationFilepath, bool overwrite)
        {
            string fixedSourceFilepath = PathHelper.FixFolderPathSeparators(sourceFilepath);
            string fixedDestinationFilepath = PathHelper.FixFolderPathSeparators(destinationFilepath);

            File.Copy(fixedSourceFilepath, fixedDestinationFilepath, overwrite);
        }

        public void WriteAllText(string filepath, string text)
        {
            string fixedFilepath = PathHelper.FixFolderPathSeparators(filepath);

            File.WriteAllText(fixedFilepath, text);
        }
    
        public string ReadAllText(string filepath)
        {
            string fixedFilepath = PathHelper.FixFolderPathSeparators(filepath);

            return File.ReadAllText(fixedFilepath);
        }

        public bool Exists(string filepath)
        {
            string fixedFilepath = PathHelper.FixFolderPathSeparators(filepath);

            return File.Exists(fixedFilepath);
        }
    }
}