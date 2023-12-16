using TicTacToe.domain.Model.DataAccess;
using TicTacToe.domain.Model.TicTacToe;

namespace TicTacToe.domain.Service
{
    public static class Extensions
    {
        public static GameView ToGameView(this Game game)
            => game is null
            ? throw new ArgumentNullException("Cannot convert null game")
            : new GameView
            {
                Id = game.Id,
                WinnerId = FindGameWinner(game) is null ? Guid.Empty : FindGameWinner(game).UserId,
                WinnerName = FindGameWinner(game) is null ? string.Empty : FindGameWinner(game).Name,
                Modified = game.Modified,
                MatchViewId = game.TicTacToeMatch.Id,
                MatchView = new MatchView
                {
                    Id = game.TicTacToeMatch.Id,
                    GameViewId = game.Id,
                    Board = game.TicTacToeMatch.Board,
                    Messages = game.Chat.Messages.Select(m => new ChatMessageView
                    {
                        Id = Guid.NewGuid(),
                        Author = m.Author,
                        When = m.When.ToUniversalTime(),
                        Content = m.Content,
                        MatchViewId = game.TicTacToeMatch.Id
                    }).ToList(),
                    State = game.TicTacToeMatch.State,
                    Modified = game.TicTacToeMatch.Modified
                },
                Users = new List<PlayerView>()
                {
                    new PlayerView
                    {
                        Id = game.Users.First().Id,
                        GameViewId = game.Id,
                        Modified = game.Users.First().Modified,
                        ConnectionId = game.Users.First().ConnectionId,
                        IsReady = false,
                        IsLoggedUser = game.Users.First().IsLoggedUser,
                        Name = game.Users.First().Name,
                        Type = game.Users.First().Type
                    },
                    new PlayerView
                    {
                        Id = game.Users.Last().Id,
                        GameViewId = game.Id,
                        Modified = game.Users.Last().Modified,
                        ConnectionId = game.Users.Last().ConnectionId,
                        IsReady = false,
                        IsLoggedUser = game.Users.Last().IsLoggedUser,
                        Name = game.Users.Last().Name,
                        Type = game.Users.Last().Type
                    }
                }
            };

        private static Player? FindGameWinner(Game game)
        {
            if (game == null) return null;

            switch (game.TicTacToeMatch.State)
            {
                case MatchState.CircleWon:
                    return game.Users.FirstOrDefault(u => u.Type == FieldType.Circle);
                case MatchState.CrossWon:
                    return game.Users.FirstOrDefault(u => u.Type == FieldType.Cross);
                case MatchState.Warmup:
                case MatchState.CircleTurn:
                case MatchState.CrossTurn:
                case MatchState.Draw:
                case MatchState.MatchInterrupted:
                default:
                    return null;
            }
        }
    }
}
