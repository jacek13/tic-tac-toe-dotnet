using TicTacToe.domain.Model.TicTacToe;

namespace TicTacToe.domain.Service
{
    public class GameService : IGameService
    {
        private List<Game> Games { get; set; } = new();

        public Game? GetGame(Guid id)
            => Games.FirstOrDefault(game => game.Id == id);

        public Game? GetGameByConnectionId(string connectionId)
            => Games.Find(game => game.Users.Exists(player => player.ConnectionId == connectionId));

        public IReadOnlyList<Game> GetActiveGames()
            => Games.Where(g => g.TicTacToeMatch.State != MatchState.Draw
                && g.TicTacToeMatch.State != MatchState.CrossWon
                && g.TicTacToeMatch.State != MatchState.CircleWon)
                .ToList();

        public Game CreateNewGame()
        {
            var newGame = new Game();
            Games.Add(newGame);
            return newGame;
        }

        public Game FindGameForUser()
            => Games.FirstOrDefault(g => g.Users.Count == 1) ?? CreateNewGame();

        public bool DeleteGameById(Guid id)
        {
            var game = Games.FirstOrDefault(g => g.Id == id);
            if (game is null) return false;

            Games.Remove(game);
            return true;
        }
    }
}
