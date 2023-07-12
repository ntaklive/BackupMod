using System;
using System.Collections.Generic;
using System.Linq;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public sealed class GameWorld : AggregateRoot
    {
        private readonly IList<GameSave> _saves;

        public GameWorld(string? name, string? directoryPath, string? hash)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            if (!Filesystem.Directory.Exists(directoryPath))
            {
                throw new ArgumentException("Directory does not exist", nameof(directoryPath));
            }

            Name = name;
            DirectoryPath = directoryPath;
            Hash = hash;

            _saves = new List<GameSave>();
        }

        public string Name { get; }

        public string DirectoryPath { get; }

        public string Hash { get; }

        public Result AddSave(GameSave save)
        {
            if (_saves.Contains(save))
            {
                return Result.Error("This save already exists in the saves list of the world");
            }
        
            _saves.Add(save);
        
            return Result.Success();
        }

        public Result<GameSave> GetSaveByName(string? name)
        {
            if (name == null)
            {
                return Result<GameSave>.Error("The game save with the specified name doesn't exist");
            }

            GameSave? save = _saves.SingleOrDefault(x => x.Name == name);
            if (save == null!)
            {
                return Result<GameSave>.Error("The save with the specified name doesn't exist");
            }

            return Result<GameSave>.Success(save);
        }
    }
}