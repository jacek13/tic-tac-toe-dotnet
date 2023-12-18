using TicTacToe.domain.Model.TicTacToe;

namespace TicTacToe.domain.Service
{
    public interface IGameService
    {
        Game CreateNewGame();

        Game FindGameForUser();

        Game? GetGame(Guid id);

        Game? GetGameByConnectionId(string connectionId);

        IReadOnlyList<Game> GetActiveGames();

        bool DeleteGameById(Guid id);

        void SetFinalState(Guid id, FieldType finalState);

        (Player, Player) AssignFieldsToPlayers(Guid gameId);

        FieldType WhichPlayerBegin(Guid gameId);

        Task<bool> SaveGameResult(Game game);
    }
}