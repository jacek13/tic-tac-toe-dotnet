﻿using TicTacToe.domain.Model.TicTacToe;

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
    }
}