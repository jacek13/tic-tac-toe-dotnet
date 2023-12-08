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
            game.Users[0].Type = FieldType.Cross;
            game.Users[1].Type = FieldType.Circle;

            game.NewMatchState(1, 1, game.Users.First().Type);
            Assert.Throws<DomainException>(() => game.NewMatchState(1, 1, game.Users.First().Type));
        }

        [Fact(DisplayName = "User One Should Win")]
        public void GameUserOneShouldWin()
        {
            var game = new Game();
            var userOne = (Guid.NewGuid(), game.TicTacToeMatch.State == MatchState.CircleTurn ? FieldType.Circle : FieldType.Cross);
            var userTwo = (Guid.NewGuid(), userOne.Item2 == FieldType.Circle ? FieldType.Cross : FieldType.Circle);

            game.AddUser(userOne.ToString());
            game.AddUser(userTwo.ToString());

            MatchState state = MatchState.Warmup;
            state = game.NewMatchState(1, 1, userOne.Item2);
            state = game.NewMatchState(0, 0, userTwo.Item2);
            state = game.NewMatchState(1, 0, userOne.Item2);
            state = game.NewMatchState(2, 0, userTwo.Item2);
            state = game.NewMatchState(1, 2, userOne.Item2);

            Assert.True(state == MatchState.CircleWon || state == MatchState.CrossWon);
        }

        [Fact(DisplayName = "User should be able to move after multiple invalid moves")]
        public void GameInvalidMove()
        {
            var game = new Game();

            game.AddUser(Guid.NewGuid().ToString());
            game.AddUser(Guid.NewGuid().ToString());
            game.Users[0].Type = FieldType.Cross;
            game.Users[1].Type = FieldType.Circle;

            MatchState state = MatchState.Warmup;
            state = game.NewMatchState(1, 1, game.Users.First().Type);
            state = game.NewMatchState(0, 0, game.Users.Last().Type);
            Assert.Throws<DomainException>(() => game.NewMatchState(1, 1, game.Users.First().Type));
            Assert.Throws<DomainException>(() => game.NewMatchState(1, 1, game.Users.First().Type));
            state = game.NewMatchState(0, 1, game.Users.First().Type);
        }

        [Fact(DisplayName = "Playes could should be able to play new game after first one")]
        public void NewGameShouldCreate()
        {
            var game = new Game();
            var userOne = (Guid.NewGuid(), game.TicTacToeMatch.State == MatchState.CircleTurn ? FieldType.Circle : FieldType.Cross);
            var userTwo = (Guid.NewGuid(), userOne.Item2 == FieldType.Circle ? FieldType.Cross : FieldType.Circle);

            game.AddUser(userOne.ToString());
            game.AddUser(userTwo.ToString());

            MatchState state = MatchState.Warmup;
            state = game.NewMatchState(1, 1, userOne.Item2);
            state = game.NewMatchState(0, 0, userTwo.Item2);
            state = game.NewMatchState(1, 0, userOne.Item2);
            state = game.NewMatchState(2, 0, userTwo.Item2);
            state = game.NewMatchState(1, 2, userOne.Item2);

            game.NewGame();
            state = game.NewMatchState(2, 1, userOne.Item2);
            state = game.NewMatchState(0, 0, userTwo.Item2);
            state = game.NewMatchState(2, 0, userOne.Item2);
            state = game.NewMatchState(2, 0, userTwo.Item2);
            state = game.NewMatchState(2, 2, userOne.Item2);

            Assert.True(state == MatchState.CircleWon || state == MatchState.CrossWon);
        }
    }
}