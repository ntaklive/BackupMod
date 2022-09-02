using System.Collections.Generic;
using BackupMod.Manifest;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface ISaveInfoFactory
{
    public SaveInfo CreateFromManifests(IReadOnlyList<BackupManifest> manifests);
}