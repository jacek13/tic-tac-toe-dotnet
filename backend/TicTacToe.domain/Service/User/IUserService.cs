using TicTacToe.domain.Model.DataAccess;
using TicTacToe.domain.Service.User.Responses;

namespace TicTacToe.domain.Service.User
{
    public interface IUserService
    {
        Task<UserInfoResponse> GetUserInfo(string accessToken);

        Task<IReadOnlyCollection<GameView>> GetAllGamesForAuthenticatedUser(string accessToken);
    }
}
