using System;
using System.Collections.Generic;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions.EqualityComparers;

public class BackupInfoEqualityComparer : IEqualityComparer<BackupInfo>
{
    public bool Equals(BackupInfo x, BackupInfo y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Title == y.Title && x.Filepath == y.Filepath && x.ManifestFilepath == y.ManifestFilepath && x.Archived == y.Archived;
    }

    public int GetHashCode(BackupInfo obj)
    {
        return HashCode.Combine(obj.Title, obj.Filepath, obj.ManifestFilepath, obj.Archived);
    }
}