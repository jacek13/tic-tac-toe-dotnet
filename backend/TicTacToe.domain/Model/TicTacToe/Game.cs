using TicTacToe.domain.Model.Common;

namespace TicTacToe.domain.Model.TicTacToe
{
    public class Game : BaseEntity
    {
        // TODO chat

        public Match TicTacToeMatch { get; set; }

        public List<string> Users { get; set; } // Trzeba by zrobić model który by miał zgody zawodników + ich identyfikatory

        public List<string> AllowNewRound { get; private set; }

        public Game()
        {
            TicTacToeMatch = new();
            TicTacToeMatch.Intrepret(new ClearBoardEvent());
            Users = new();
            AllowNewRound = new();
        }

        public bool AddUser(string userConnection)
        {
            if (Users.Count >= 2)
                return false;

            Users.Add(userConnection);
            return true;
        }

        public bool UserAllowedNewRound(string callerId)
        {
            if (AllowNewRound.Count == 2)
                return true;

            if (!AllowNewRound.Exists(id => id == callerId))
                AllowNewRound.Add(callerId);

            return AllowNewRound.Count == 2;
        }

        public void NewGame()
        {
            TicTacToeMatch = new();
            TicTacToeMatch.Intrepret(new ClearBoardEvent());
            AllowNewRound.Clear();
        }

        public FieldType WhichPlayerBegin()
            => TicTacToeMatch.State switch
            {
                MatchState.CircleTurn => FieldType.Circle,
                MatchState.CrossTurn => FieldType.Cross,
                _ => FieldType.None
            };

        public MatchState NewMatchState(int x, int y, FieldType who)
        {
            TicTacToeMatch.Intrepret(new MoveEvent(((uint)x, (uint)y), who));
            TicTacToeMatch.Intrepret(new CheckIfDrawEvent());
            TicTacToeMatch.Intrepret(new CheckWonEvent(FieldType.Circle));
            TicTacToeMatch.Intrepret(new CheckWonEvent(FieldType.Cross));
            return TicTacToeMatch.State;
        }
    }
}
