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
            var game = _gameService.CreateNewGame();
            return game.Id;
        }

        [HttpGet("/games/find-random")]
        public Guid FindRandomGame()
        {
            var game = _gameService.FindGameForUser();
            return game.Id;
        }

        [HttpGet("/games")]
        public IReadOnlyList<Game> GetActiveGames()
            => _gameService.GetActiveGames();
    }
}
