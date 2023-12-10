using TicTacToe.domain.Model.DataAccess;

namespace TicTacToe.domain.Service.GameHistory
{
    public interface IGameHistoryService
    {
        Task<GameView?> GetGame(Guid id);

        Task<IReadOnlyCollection<GameView>> GetAllGames();

        Task<IReadOnlyCollection<GameView>> GetAllGamesForUser(Guid userId);

        Task<bool> StoreGame(GameView game);
    }
}
