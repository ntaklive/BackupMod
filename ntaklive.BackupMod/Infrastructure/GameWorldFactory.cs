using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Domain;
using ntaklive.BackupMod.Utils;

using static PathAbstractions;

namespace ntaklive.BackupMod.Infrastructure
{
    public class GameWorldFactory : IGameWorldFactory
    {
        private readonly IGameSaveFactory _gameSaveFactory;
        private readonly ILogger<GameWorldFactory>? _logger;

        public GameWorldFactory(IGameSaveFactory gameSaveFactory, ILogger<GameWorldFactory>? logger)
        {
            _gameSaveFactory = gameSaveFactory;
            _logger = logger;
        }
    
        public Result<IList<GameWorld>> LoadAvailableWorlds()
        {
            try
            {
                var worldsList = new List<GameWorld>();
                foreach (AbstractedLocation location in WorldsSearchPaths.GetAvailablePathsList())
                {
                    string checksumsTxtFilepath = Filesystem.Path.Combine(location.FullPath, Constants.cFileWorldChecksums);
                    if (!Filesystem.File.Exists(checksumsTxtFilepath))
                    {
                        _logger?.LogWarning("Unable to find {ChecksumsTxtFilepath}", checksumsTxtFilepath);
                        continue;
                    }
            
                    string hash = Md5HashHelper.ComputeTextHash(checksumsTxtFilepath);
                    string worldName = Filesystem.Directory.GetDirectoryName(location.FullPath);
                    string worldDirectoryPath = location.FullPath;

                    var world = new GameWorld(worldName, worldDirectoryPath, hash);
                    Result<IList<GameSave>> getWorldSavesResult = _gameSaveFactory.LoadAvailableSaves(world);
                    if (!getWorldSavesResult.IsSuccess)
                    {
                        return Result<IList<GameWorld>>.Error(getWorldSavesResult.ErrorMessage);
                    }

                    worldsList.Add(world);
                }
            
                return Result<IList<GameWorld>>.Success(worldsList);
            }
            catch (Exception exception)
            {
                return Result<IList<GameWorld>>.Error(exception.Message);
            }
        }
    }
}