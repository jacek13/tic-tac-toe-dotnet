using TicTacToe.domain.Model.TicTacToe;
using TicTacToe.domain.Service.GameHistory;

namespace TicTacToe.domain.Service
{
    public class GameService : IGameService
    {
        private readonly IGameHistoryService _gameHistoryService;

        private List<Game> Games { get; set; } = new();

        public GameService(IGameHistoryService gameHistoryService)
        {
            _gameHistoryService = gameHistoryService;
        }

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

        public async Task<bool> SaveGameResult(Guid id)
        {
            var game = Games.FirstOrDefault(g => g.Id == id);
            if (game is null) return false;

            var gameView = game.ToGameView();
            return await _gameHistoryService.StoreGame(gameView);
        }

        public void SetFinalState(Guid id, FieldType finalState)
        {
            var game = Games.FirstOrDefault(g => g.Id == id);
            if (game is null) return;

            game.WinnerField = finalState;
        }
    }
}
