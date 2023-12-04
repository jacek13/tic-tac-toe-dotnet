using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Text.Json;
using TicTacToe.domain.Model.TicTacToe;
using TicTacToe.domain.Service;

namespace TicTacToe.webapi.Hubs
{
    public class GameHub : Hub
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<GameHub>();

        private readonly IGameService _gameService;

        public GameHub(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task PlayerJoinGame(Guid gameId)
        {
            var game = _gameService.GetGame(gameId);

            if (game is null)
            {
                _logger.Warning("Could not found game with id: {gameId}", gameId);
                await Clients.Caller.SendAsync("Error", $"The requested game with id: {gameId} don't exists!");
                return;
            }

            game.AddUser(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());
            _logger.Information("Added user with connectionId: {connectionId}", Context.ConnectionId);

            if (game.Users.Count == 2)
            {
                await StartGame(game);
            }
        }

        async Task StartGame(Game game)
        {
            _logger.Information("Starting the game with id: {gameId}", game.Id);

            bool isPlayerOneBeginner = game.WhichPlayerBegin() == FieldType.Circle;

            await Clients.Client(game.Users[0]).SendAsync("SetMover", isPlayerOneBeginner);
            await Clients.Client(game.Users[0]).SendAsync("SetChar", isPlayerOneBeginner ? 'X' : 'O');

            await Clients.Client(game.Users[1]).SendAsync("SetMover", !isPlayerOneBeginner);
            await Clients.Client(game.Users[1]).SendAsync("SetChar", !isPlayerOneBeginner ? 'X' : 'O');

            await Clients.Group(game.Id.ToString()).SendAsync("GetGame", JsonSerializer.Serialize(game.TicTacToeMatch.Board));
        }

        public async Task PlayerMove(Guid gameId, int x, int y, char who)
        {
            var game = _gameService.GetGame(gameId);

            try
            {
                if (game is null)
                {
                    await Clients.Caller.SendAsync("Error", "Game couldn't be found");
                    return;
                }

                var currentState = game.TicTacToeMatch.State;
                var stateAfterMove = game.NewMatchState(x, y, who == 'X' ? FieldType.Cross : FieldType.Circle);

                await Clients.Group(game.Id.ToString()).SendAsync("UpdateBoard", JsonSerializer.Serialize(game.TicTacToeMatch.Board));

                switch (stateAfterMove)
                {
                    case MatchState.CircleTurn:
                        await Clients.Client(game.Users.FindAll(connId => connId != Context.ConnectionId)[0]).SendAsync("SetMover", true);
                        break;
                    case MatchState.CrossTurn:
                        await Clients.Caller.SendAsync("SetMover", true);
                        break;
                    case MatchState.Draw:
                        await Clients.Group(game.Id.ToString()).SendAsync("GameEnded", "DRAW");
                        break;
                    case MatchState.CircleWon:
                        await Clients.Group(game.Id.ToString()).SendAsync("GameEnded", "CIRCLE_WON");
                        break;
                    case MatchState.CrossWon:
                        await Clients.Group(game.Id.ToString()).SendAsync("GameEnded", "CROSS_WON");
                        break;
                    case MatchState.MatchInterrupted:
                    default:
                        await Clients.Group(game.Id.ToString()).SendAsync("ERROR", "Something went wrong");
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.Warning("Something went wrong: {message}", e.Message);
                await Clients.Group(game.Id.ToString()).SendAsync("ERROR", "Something went wrong");
                return;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);

            var game = _gameService.GetGameByConnectionId(Context.ConnectionId);
            if (game is null) return;

            if (game.Users.Any(u => u == Context.ConnectionId))
            {
                game.Users.Remove(Context.ConnectionId);
            }

            if (game.Users.Count == 0)
                _gameService.Games.Remove(game);
        }
    }
}
