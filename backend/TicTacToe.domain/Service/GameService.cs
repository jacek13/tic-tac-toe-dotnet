using TicTacToe.domain.Model.TicTacToe;

namespace TicTacToe.domain.Service
{
    public class GameService : IGameService
    {
        public List<Game> Games { get; private set; } = new();

        public Game GetGame(Guid id)
        {
            return Games.Find(game => game.Id == id);
        }

        public Game GetGameByConnectionId(string connId)
        {
            return Games.Find(game => game.Users.Exists(id => id == connId));
        }

        public IReadOnlyList<Game> GetActiveGames()
        {
            return Games.Where(g => g.TicTacToeMatch.State != MatchState.Draw
                && g.TicTacToeMatch.State != MatchState.CrossWon
                && g.TicTacToeMatch.State != MatchState.CircleWon)
                .ToList();
        }
    }
}
