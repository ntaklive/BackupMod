using System.Collections.Generic;
using BackupMod.Manifest;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IWorldInfoFactory
{
    public WorldInfo CreateFromManifests(IReadOnlyList<BackupManifest> manifests);
}