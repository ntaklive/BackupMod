using System;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Application.Commands.Args;
using ntaklive.BackupMod.Domain;
using ntaklive.BackupMod.Infrastructure;

namespace ntaklive.BackupMod.Application.Commands
{
    public class BackupCommand : IConsoleCommand<BackupCommandArgs>
    {
        private readonly Game _game;
        private readonly IGameSaveBackingUpAlgorithm _gameSaveBackingUpAlgorithm;

        public BackupCommand(Game game, IGameSaveBackingUpAlgorithm gameSaveBackingUpAlgorithm)
        {
            _game = game;
            _gameSaveBackingUpAlgorithm = gameSaveBackingUpAlgorithm;
        }
    
        public Result CanExecute(BackupCommandArgs args)
        {
            if (!_game.IsStarted())
            {
                return Result.Error(Localization.Get(LocalizationKeys.OnlyWhenTheGameIsStarted));
            }
        
            return Result.Success();
        }

        public Result Execute(BackupCommandArgs args)
        {
            try
            {
                Result canExecuteResult = CanExecute(args);
                if (!canExecuteResult.IsSuccess)
                {
                    return Result.Error(canExecuteResult.ErrorMessage);
                }

                ValueResult<(Backup backup, TimeSpan timeSpent)> backUpResult = _gameSaveBackingUpAlgorithm.BackUpActiveSave(args.Title, BackupCaller.Command);
                if (!backUpResult.IsSuccess)
                {
                    return Result.Error(backUpResult.ErrorMessage);
                }

                return Result.Success();
            }
            catch (Exception exception)
            {
                return Result.Error(exception.Message);
            }
        }
    }
}