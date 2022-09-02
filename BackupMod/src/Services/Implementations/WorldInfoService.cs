using System.Collections.Generic;
using System.IO;
using System.Linq;
using BackupMod.Manifest;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

public class WorldInfoService : IWorldInfoService
{
    private readonly IFilesystem _filesystem;
    private readonly IResources _resources;
    private readonly IBackupManifestService _manifestService;
    private readonly IWorldInfoFactory _worldInfoFactory;

    public WorldInfoService(
        IFilesystem filesystem,
        IResources resources,
        IBackupManifestService manifestService,
        IWorldInfoFactory worldInfoFactory)
    {
        _filesystem = filesystem;
        _resources = resources;
        _manifestService = manifestService;
        _worldInfoFactory = worldInfoFactory;
    }

    public IReadOnlyList<WorldInfo> GetWorldInfos()
    {
        string backupsDirectoryPath = _resources.GetBackupsDirectoryPath();
        string archiveDirectoryPath = _resources.GetArchiveDirectoryPath();

        string[] manifestPaths = Enumerable.Concat(
                _filesystem.Directory.EnumerateFiles(backupsDirectoryPath,
                    $"*{ModConfiguration.Constants.BackupManifestExtension}",
                    SearchOption.AllDirectories).AsEnumerable(),
                _filesystem.Directory.EnumerateFiles(archiveDirectoryPath,
                    $"*{ModConfiguration.Constants.BackupManifestExtension}",
                    SearchOption.AllDirectories).AsEnumerable())
            .ToArray();

        BackupManifest[] manifests = manifestPaths.Select(path => _manifestService.ReadManifest(path)).ToArray();

        var worlds = new List<WorldInfo>();
        foreach (IGrouping<string, BackupManifest> group in manifests.GroupBy(x => x.World.Name))
        {
            if (worlds.Exists(x => x.Name == group.Key))
            {
                continue;
            }

            WorldInfo worldInfo = _worldInfoFactory.CreateFromManifests(
                group.Select(x => x)
                    .Where(x => x != null)
                    .ToArray()
            );

            if (worldInfo == null)
            {
                continue;
            }
            
            worlds.Add(worldInfo);
        }

        return worlds;
    }
}