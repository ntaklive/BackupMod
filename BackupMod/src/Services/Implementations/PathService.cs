using System.IO;
using BackupMod.Services.Abstractions;
using BackupMod.Utils;

namespace BackupMod.Services;

public class PathService : IPathService
{
    public string GetDirectoryName(string path)
    {
        return Path.GetDirectoryName(path);
    }

    public string GetFileNameWithoutExtension(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    public string Combine(string arg1, string arg2)
    {
        return PathHelper.FixFolderPathSeparators(Path.Combine(arg1, arg2));
    }

    public string Combine(string arg1, string arg2, string arg3)
    {
        return PathHelper.FixFolderPathSeparators(Path.Combine(arg1, arg2, arg3));
    }

    public string Combine(string arg1, string arg2, string arg3, string arg4)
    {
        return PathHelper.FixFolderPathSeparators(Path.Combine(arg1, arg2, arg3, arg4));
    }
}