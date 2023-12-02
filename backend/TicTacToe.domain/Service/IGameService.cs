using TicTacToe.domain.Model.TicTacToe;

namespace TicTacToe.domain.Service
{
    public interface IGameService
    {
        List<Game> Games { get; }

        Game GetGame(Guid id);

        Game GetGameByConnectionId(string connId);

        IReadOnlyList<Game> GetActiveGames();
    }
}