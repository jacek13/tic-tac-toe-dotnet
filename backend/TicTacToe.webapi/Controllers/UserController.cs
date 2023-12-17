using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.domain.Model.DataAccess;
using TicTacToe.domain.Service.User;
using TicTacToe.domain.Service.User.Responses;

namespace TicTacToe.webapi.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("user/info")]
        [Authorize]
        public async Task<UserInfoResponse> GetAuthenticatedUserInfo()
        {
            var accessToken = HttpContext?.Request?.Headers["Authorization"].ToString()?.Substring(7);
            return await _userService.GetUserInfo(accessToken);
        }

        [HttpGet("user/history-games")]
        [Authorize]
        public async Task<IReadOnlyCollection<GameView>> GetAuthenticatedUserHistoryGames()
        {
            var accessToken = HttpContext?.Request?.Headers["Authorization"].ToString()?.Substring(7);
            return await _userService.GetAllGamesForAuthenticatedUser(accessToken);
        }

        [HttpGet("user/score-board")]
        public async Task<IReadOnlyCollection<UserGameHistoryStats>> GetGlobalScoreBoard()
        {
            return await _userService.GetAllGamesForAuthenticatedUsers();
        }
    }
}
