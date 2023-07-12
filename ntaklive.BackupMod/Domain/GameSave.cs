using System;
using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;

namespace ntaklive.BackupMod.Domain
{
    public class GameSave : AggregateRoot
    {
        private readonly IList<Backup> _backups;

        public GameSave(string? name, string? directoryPath, GameWorld? world)
        {
            if (name == null!)
            {
                throw new ArgumentNullException(nameof(name));
            }       
            if (directoryPath == null!)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }       
            if (world! == null!)
            {
                throw new ArgumentNullException(nameof(world));
            }

            Name = name;
            DirectoryPath = directoryPath;
            World = world;
        
            _backups = new List<Backup>();
        }

        public string DirectoryPath { get; }

        public string Name { get; }

        public GameWorld World { get; }
    
        public Result AddBackup(Backup backup)
        {
            if (_backups.Contains(backup))
            {
                return Result.Error("This backup already exists in the backups list of the save");
            }
        
            _backups.Add(backup);
        
            return Result.Success();
        }
    
        public Result RemoveBackup(Backup backup)
        {
            if (!_backups.Contains(backup))
            {
                return Result.Error("This backup doesn't exist in the backups list of the save");
            }
        
            _backups.Remove(backup);
        
            return Result.Success();
        }
    }
}