using System.IO;

namespace BackupMod.Utilities;

public static class PathHelper
{
    public static char DirectorySeparatorChar => Path.AltDirectorySeparatorChar;
    
    public static string FixFolderPathSeparators(string folderPath)
    {
        return folderPath
            .Replace('\\', Path.AltDirectorySeparatorChar)
            .Replace('/', Path.AltDirectorySeparatorChar)
            .Replace("//", Path.AltDirectorySeparatorChar.ToString());
    }
}