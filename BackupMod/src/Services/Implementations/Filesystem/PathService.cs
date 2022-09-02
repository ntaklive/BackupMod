using System.IO;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Utils;

namespace BackupMod.Services.Filesystem;

public class PathService : IPathService
{
    public string GetDirectoryName(string path)
    {
        return Path.GetDirectoryName(PathHelper.FixFolderPathSeparators(path));
    }

    public string GetFileNameWithoutExtension(string path)
    {
        return Path.GetFileNameWithoutExtension(PathHelper.FixFolderPathSeparators(path));
    }

    public string Combine(string arg1, string arg2)
    {
        return PathHelper.FixFolderPathSeparators(Path.Combine(PathHelper.FixFolderPathSeparators(arg1), PathHelper.FixFolderPathSeparators(arg2)));
    }

    public string Combine(string arg1, string arg2, string arg3)
    {
        return Combine(Combine(arg1, arg2), arg3);
    }

    public string Combine(string arg1, string arg2, string arg3, string arg4)
    {
        return Combine(Combine(Combine(arg1, arg2), arg3), arg4);
    }
}