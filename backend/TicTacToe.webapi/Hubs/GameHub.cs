using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Text.Json;
using TicTacToe.domain;
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

            if (game.Users.Count >= 2)
            {
                await SendAsyncToClient(Context.ConnectionId, "Error", "Game room is full");
                return;
            }

            game.AddUser(Context.ConnectionId);
            game.AddMessage($"[Game room: {game.Id}]", "player joined room");

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());
            await SendAsyncToGroup(game.Id.ToString(), "NewChatMessage", game.GetLastMessage());
            _logger.Information("Added user with connectionId: {connectionId}", Context.ConnectionId);

            if (game.Users.Count == 2)
            {
                await StartGame(game);
            }
        }

        private async Task StartGame(Game game)
        {
            _logger.Information("Starting the game with id: {gameId}", game.Id);

            var (playerFirst, playerSecond) = game.AssignFieldsToPlayers();
            var startingField = game.WhichPlayerBegin();
            var isPlayerOneBeginner = playerFirst.Type == startingField;

            await SendAsyncToClient(playerFirst.ConnectionId, "SetMover", isPlayerOneBeginner);
            await SendAsyncToClient(playerFirst.ConnectionId, "SetChar", isPlayerOneBeginner ? 'X' : 'O');
            await SendAsyncToClient(playerSecond.ConnectionId, "SetMover", !isPlayerOneBeginner);
            await SendAsyncToClient(playerSecond.ConnectionId, "SetChar", !isPlayerOneBeginner ? 'X' : 'O');
            await SendAsyncToGroup(game.Id.ToString(), "GetGame", JsonSerializer.Serialize(game.GetBoardAsCharacters()));

            game.Chat.Add($"[Game room: {game.Id}]", $"First move for: {game.FieldTypeToChar(startingField)}");
            await SendAsyncToGroup(game.Id.ToString(), "NewChatMessage", game.GetLastMessage());
        }

        public async Task SendChatMessage(Guid gameId, string message)
        {
            var game = _gameService.GetGame(gameId);
            if (game is null)
            {
                await Clients.Caller.SendAsync("Error", "Game couldn't be found");
                return;
            }

            var player = game.Users.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (player is null)
            {
                await Clients.Caller.SendAsync("Error", "Player couldn't be found");
                return;
            }

            game.Chat.Add(player.Name, message);
            await SendAsyncToGroup(game.Id.ToString(), "NewChatMessage", game.GetLastMessage());
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

                await SendAsyncToGroup(game.Id.ToString(), "UpdateBoard", JsonSerializer.Serialize(game.GetBoardAsCharacters()));
                var message = (string.Empty, string.Empty);

                switch (stateAfterMove)
                {
                    case MatchState.CircleTurn:
                    case MatchState.CrossTurn:
                        var player = game.Users.FirstOrDefault(player => player.IsPlayerTurn(stateAfterMove));
                        await SendAsyncToClient(player!.ConnectionId, "SetMover", true);
                        break;
                    case MatchState.Draw:
                        await SendAsyncToGroup(game.Id.ToString(), "GameEnded", "DRAW");
                        message = ($"[Game room: {game.Id}]", $"Game ended: {MatchState.Draw}");
                        await _gameService.SaveGameResult(game.Id);
                        break;
                    case MatchState.CircleWon:
                        await SendAsyncToGroup(game.Id.ToString(), "GameEnded", "CIRCLE_WON");
                        message = ($"[Game room: {game.Id}]", $"Game ended: {MatchState.CircleWon}");
                        await _gameService.SaveGameResult(game.Id);
                        break;
                    case MatchState.CrossWon:
                        await SendAsyncToGroup(game.Id.ToString(), "GameEnded", "CROSS_WON");
                        message = ($"[Game room: {game.Id}]", $"Game ended: {MatchState.CrossWon}");
                        await _gameService.SaveGameResult(game.Id);
                        break;
                    case MatchState.MatchInterrupted:
                    default:
                        await SendAsyncToGroup(game.Id.ToString(), "ERROR", "Something went wrong");
                        break;
                }

                if (message != (string.Empty, string.Empty))
                {
                    game.Chat.Add(message.Item1, message.Item2);
                    await SendAsyncToGroup(game.Id.ToString(), "NewChatMessage", game.GetLastMessage());
                }
            }
            catch (DomainException e)
            {
                _logger.Warning("Domain error: {message}", e.Message);
                game.Chat.Add($"[Game room: {game.Id}]", e.Message);
                await SendAsyncToGroup(game.Id.ToString(), "NewChatMessage", game.GetLastMessage());
            }
            catch (Exception e)
            {
                _logger.Warning("Something went wrong: {message}", e.Message);
                await SendAsyncToGroup(game.Id.ToString(), "ERROR", "Something went wrong");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);

            var game = _gameService.GetGameByConnectionId(Context.ConnectionId);
            if (game is null) return;

            if (game.Users.Any(u => u.ConnectionId == Context.ConnectionId))
            {
                var disconectedPlayer = game.Users.First(u => u.ConnectionId == Context.ConnectionId);
                game.Users.Remove(disconectedPlayer);
                game.AddMessage($"[Game room: {game.Id}]", "player leaved game");
                await SendAsyncToGroup(game.Id.ToString(), "NewChatMessage", game.GetLastMessage());
            }

            if (game.Users.Count == 0)
                _gameService.DeleteGameById(game.Id);
        }

        private async Task SendAsyncToGroup(string groupName, string method, object? argument)
            => await Clients.Group(groupName).SendAsync(method, argument);

        private async Task SendAsyncToClient(string connectionId, string method, object? argument)
            => await Clients.Client(connectionId).SendAsync(method, argument);
    }
}
