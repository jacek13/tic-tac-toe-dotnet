using Microsoft.AspNetCore.Mvc;
using TicTacToe.domain.Model.TicTacToe;
using TicTacToe.domain.Service;

namespace TicTacToe.webapi.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("/games")]
        public Guid CreateGame()
        {
            // TODO validation
            // TODO more methods instead of _gameService reference
            var game = new Game();

            _gameService.Games.Add(game);
            return game.Id;
        }

        [HttpGet("/games")]
        public IReadOnlyList<Game> GetActivegames()
            => _gameService.GetActiveGames();
    }
}
