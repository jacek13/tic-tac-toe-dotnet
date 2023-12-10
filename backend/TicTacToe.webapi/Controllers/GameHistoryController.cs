using Microsoft.AspNetCore.Mvc;
using TicTacToe.domain.Model.DataAccess;
using TicTacToe.domain.Service.GameHistory;

namespace TicTacToe.webapi.Controllers
{
    [ApiController]
    public class GameHistoryController : ControllerBase
    {
        private IGameHistoryService _gameHistoryService;

        public GameHistoryController(IGameHistoryService gameHistoryService)
        {
            _gameHistoryService = gameHistoryService;
        }

        [HttpGet("/history-games")]
        public async Task<IReadOnlyCollection<GameView>> GetAllHistoryGames()
        {
            return await _gameHistoryService.GetAllGames();
        }

        [HttpGet("/history-games/user/{id:guid}")]
        public async Task<IReadOnlyCollection<GameView>> GetAllHistoryGamesForUser(Guid id)
        {
            return await _gameHistoryService.GetAllGamesForUser(id);
        }
    }
}
