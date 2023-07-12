namespace ntaklive.BackupMod
{
    public interface IFileService
    {
        public void Delete(string filepath);
    
        public void Copy(string sourceFilepath, string destinationFilepath, bool overwrite);

        public void WriteAllText(string filepath, string text);

        public string ReadAllText(string filepath);

        public bool Exists(string filepath);
    }
}