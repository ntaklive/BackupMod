using System.Collections.Generic;
using System.Linq;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Utils;

namespace ntaklive.BackupMod.Domain
{
    public sealed class Game : AggregateRoot
    {
        private IList<GameWorld> _worlds;

        public Game()
        {
            ActiveSave = null;
            _worlds = new List<GameWorld>();
        }

        public string SavesDirectoryPath =>
            PathHelper.FixFolderPathSeparators(GamePrefs.GetString(EnumGamePrefs.SaveGameFolder));

        public string WorldsDirectoryPath => 
            Filesystem.Path.Combine(GameIO.GetApplicationPath(), "Worlds");

        public GameSave? ActiveSave { get; private set; }

        public Result CanStart(GameSave? save)
        {
            if (save! == null!)
            {
                return Result.Error("The save cannot be null");
            }
            
            if (IsStarted())
            {
                return Result.Error("The game has already started");
            }

            World world = GameManager.Instance.World;
            if (world != null && !world.IsRemote())
            {
                return Result.Error("You're not the host of the server");
            }

            return Result.Success();
        }

        public Result Start(GameSave? save)
        {
            if (save! == null!)
            {
                return Result.Error("The save cannot be null");
            }
            
            Result canStartResult = CanStart(save);
            if (!canStartResult.IsSuccess)
            {
                return Result.Error(canStartResult.ErrorMessage);
            }

            ActiveSave = save;

            return Result.Success();
        }

        public Result CanExit()
        {
            if (!IsStarted())
            {
                return Result.Error("The game is not started yet or you're not the host of the server");
            }

            return Result.Success();
        }

        public Result Exit()
        {
            Result canExitResult = CanExit();
            if (!canExitResult.IsSuccess)
            {
                return Result.Error(canExitResult.ErrorMessage);
            }

            ActiveSave = null;

            return Result.Success();
        }

        public bool IsStarted() => ActiveSave! != null!;

        public Result UpdateWorldsList(IList<GameWorld>? worlds)
        {
            if (worlds == null!)
            {
                return Result.Error("The worlds list cannot be null");
            }

            _worlds = worlds;

            return Result.Success();
        }

        public Result<GameWorld> GetWorldByNameAndHash(string? name, string? hash)
        {
            if (name == null)
            {
                return Result<GameWorld>.Error("The world name cannot be null");
            }

            if (hash == null)
            {
                return Result<GameWorld>.Error("The world hash cannot be null");
            }

            GameWorld? world = _worlds.SingleOrDefault(x => x.Name == name && x.Hash == hash);
            if (world == null!)
            {
                return Result<GameWorld>.Error("The world with the specified name doesn't exist");
            }

            return Result<GameWorld>.Success(world);
        }
    }
}