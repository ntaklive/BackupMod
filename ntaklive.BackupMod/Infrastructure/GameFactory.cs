using System.Collections.Generic;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Infrastructure
{
    public class GameFactory : IGameFactory
    {
        private readonly IGameWorldFactory _gameWorldFactory;

        public GameFactory(IGameWorldFactory gameWorldFactory)
        {
            _gameWorldFactory = gameWorldFactory;
        }
    
        public Result<Domain.Game> CreateGame()
        {
            Result<IList<GameWorld>> getAvailableWorldsResult = _gameWorldFactory.LoadAvailableWorlds();
            if (!getAvailableWorldsResult.IsSuccess)
            {
                return Result<Domain.Game>.Error(getAvailableWorldsResult.ErrorMessage);
            }
            IList<GameWorld> worlds = getAvailableWorldsResult.Data!;
        
            var game = new Domain.Game();
            Result updateWorldsListResult = game.UpdateWorldsList(worlds);
            if (!updateWorldsListResult.IsSuccess)
            {
                return Result<Domain.Game>.Error(updateWorldsListResult.ErrorMessage);
            }

            return Result<Domain.Game>.Success(game);
        }
    }
}