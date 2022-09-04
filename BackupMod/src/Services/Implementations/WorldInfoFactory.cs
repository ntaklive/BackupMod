using System.Collections.Generic;
using System.Linq;
using BackupMod.Manifest;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services;

public class WorldInfoFactory : IWorldInfoFactory
{
    private readonly IFilesystem _filesystem;
    private readonly IResources _resources;
    private readonly ISaveInfoFactory _saveInfoFactory;
    private readonly ILogger<WorldInfoFactory> _logger;

    public WorldInfoFactory(
        IFilesystem filesystem,
        IResources resources,
        ISaveInfoFactory saveInfoFactory,
        ILogger<WorldInfoFactory> logger)
    {
        _filesystem = filesystem;
        _resources = resources;
        _saveInfoFactory = saveInfoFactory;
        _logger = logger;
    }

    [CanBeNull]
    public WorldInfo CreateFromManifests(IReadOnlyList<BackupManifest> manifests)
    {
        var saves = new List<SaveInfo>();
        foreach (IGrouping<string, BackupManifest> group in manifests.GroupBy(x => x.Save.Name))
        {
            if (saves.Exists(x => x.Name == group.Key))
            {
                continue;
            }

            SaveInfo saveInfo = _saveInfoFactory.CreateFromManifests(
                group.Select(x => x)
                    .Where(x => x != null)
                    .ToArray()
            );
            
            if (saveInfo == null)
            {
                continue;
            }
            
            saves.Add(saveInfo);
        }

        WorldInfo worldInfo = null;
        foreach (BackupManifest worldManifest in manifests)
        {
            if (worldInfo != null)
            {
                break;
            }
            
            string worldName = worldManifest.World.Name;
            string localWorldDirectoryPath = _filesystem.Path.Combine(
                _resources.GetWorldsDirectoryPath(),
                worldName);

            string localWorldMd5Hash = _resources.GetMd5HashForWorld(worldName);
                
            if (worldManifest.World.Md5Hash != localWorldMd5Hash)
            {
                _logger.LogWarning(
                    "The world named {WorldName} has a different MD5hash, than the one specified in the {ManifestFilepath} manifest",
                    worldManifest.World.Name, worldManifest.Filepath);
                _logger.LogWarning("Current: {CurrentMd5Hash}", localWorldMd5Hash);
                _logger.LogWarning("Required: {RequiredMd5Hash}", worldManifest.World.Md5Hash);
                continue;
            }
            
            worldInfo = new WorldInfo
            {
                Name = worldName,
                Md5Hash = localWorldMd5Hash,
                WorldDirectoryPath = localWorldDirectoryPath,
                Saves = saves
            };
        }
        
        return worldInfo;
    }
}