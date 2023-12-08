using TicTacToe.domain;
using TicTacToe.domain.Model.TicTacToe;

namespace TicTacToe.test
{
    public class UnitTestGameLogic
    {
        [Fact(DisplayName = "Match state should change after move")]
        public void GameAfterMoveStateShouldChange()
        {
            var game = new Game();

            game.AddUser(Guid.NewGuid().ToString());
            game.AddUser(Guid.NewGuid().ToString());

            var (playerFirst, playerSecond) = game.AssignFieldsToPlayers();
            var isPlayerOneBeginner = playerFirst.Type == game.WhichPlayerBegin();

            game.NewMatchState(1, 1, playerFirst.Type);
            Assert.Throws<DomainException>(() => game.NewMatchState(1, 1, playerFirst.Type));
        }

        [Fact(DisplayName = "User One Should Win")]
        public void GameUserOneShouldWin()
        {
            var game = new Game();

            game.AddUser(Guid.NewGuid().ToString());
            game.AddUser(Guid.NewGuid().ToString());

            var (playerFirst, playerSecond) = game.AssignFieldsToPlayers();
            var isPlayerOneBeginner = playerFirst.Type == game.WhichPlayerBegin();

            MatchState state = game.TicTacToeMatch.State;
            state = game.NewMatchState(1, 1, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);
            state = game.NewMatchState(0, 0, isPlayerOneBeginner ? playerSecond.Type : playerFirst.Type);
            state = game.NewMatchState(1, 0, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);
            state = game.NewMatchState(2, 0, isPlayerOneBeginner ? playerSecond.Type : playerFirst.Type);
            state = game.NewMatchState(1, 2, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);

            Assert.True(state == MatchState.CircleWon || state == MatchState.CrossWon);
        }

        [Fact(DisplayName = "User should be able to move after multiple invalid moves")]
        public void GameInvalidMove()
        {
            var game = new Game();

            game.AddUser(Guid.NewGuid().ToString());
            game.AddUser(Guid.NewGuid().ToString());

            var (playerFirst, playerSecond) = game.AssignFieldsToPlayers();
            var isPlayerOneBeginner = playerFirst.Type == game.WhichPlayerBegin();

            MatchState state = MatchState.Warmup;
            state = game.NewMatchState(1, 1, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);
            state = game.NewMatchState(0, 0, isPlayerOneBeginner ? playerSecond.Type : playerFirst.Type);
            Assert.Throws<DomainException>(() => game.NewMatchState(1, 1, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type));
            Assert.Throws<DomainException>(() => game.NewMatchState(1, 1, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type));
            state = game.NewMatchState(1, 0, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);
            state = game.NewMatchState(0, 1, isPlayerOneBeginner ? playerSecond.Type : playerFirst.Type);
            state = game.NewMatchState(1, 2, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);
            Assert.True(state == MatchState.CircleWon || state == MatchState.CrossWon);
        }

        [Fact(DisplayName = "Playes should be able to play new game after first one")]
        public void NewGameShouldCreate()
        {
            var game = new Game();

            game.AddUser(Guid.NewGuid().ToString());
            game.AddUser(Guid.NewGuid().ToString());

            var (playerFirst, playerSecond) = game.AssignFieldsToPlayers();
            var isPlayerOneBeginner = playerFirst.Type == game.WhichPlayerBegin();

            MatchState state = MatchState.Warmup;
            state = game.NewMatchState(1, 1, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);
            state = game.NewMatchState(0, 0, isPlayerOneBeginner ? playerSecond.Type : playerFirst.Type);
            state = game.NewMatchState(1, 0, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);
            state = game.NewMatchState(2, 0, isPlayerOneBeginner ? playerSecond.Type : playerFirst.Type);
            state = game.NewMatchState(1, 2, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);

            game.NewGame();

            (playerFirst, playerSecond) = game.AssignFieldsToPlayers();
            isPlayerOneBeginner = playerFirst.Type == game.WhichPlayerBegin();

            state = game.NewMatchState(2, 1, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);
            state = game.NewMatchState(0, 0, isPlayerOneBeginner ? playerSecond.Type : playerFirst.Type);
            state = game.NewMatchState(2, 0, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);
            state = game.NewMatchState(1, 0, isPlayerOneBeginner ? playerSecond.Type : playerFirst.Type);
            state = game.NewMatchState(2, 2, isPlayerOneBeginner ? playerFirst.Type : playerSecond.Type);

            Assert.True(state == MatchState.CircleWon || state == MatchState.CrossWon);
        }
    }
}