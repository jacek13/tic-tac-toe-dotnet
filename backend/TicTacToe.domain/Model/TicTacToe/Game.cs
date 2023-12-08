using TicTacToe.domain.Model.Chat;
using TicTacToe.domain.Model.Common;

namespace TicTacToe.domain.Model.TicTacToe
{
    public class Game : BaseEntity
    {
        public GameChat Chat { get; set; }

        public Match TicTacToeMatch { get; set; }

        public List<Player> Users { get; set; }

        public List<string> AllowNewRound { get; private set; }

        public Game()
        {
            Chat = new();
            TicTacToeMatch = new();
            TicTacToeMatch.Intrepret(new ClearBoardEvent());
            Users = new();
            AllowNewRound = new();
        }

        public bool AddUser(string userConnection, string name = default, Guid userId = default)
        {
            if (Users.Count >= 2)
                return false;

            var player = string.IsNullOrWhiteSpace(name) || userId == Guid.Empty
                ? new Player(userConnection)
                : new Player(userConnection, name, userId);

            Users.Add(player);
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

        public (Player, Player) AssignFieldsToPlayers()
        {
            if (Users is null) throw new ArgumentNullException("Users cannot be null");
            if (Users.Count < 2) throw new DomainException(DomainError.InvalidPlayerCount);

            switch (WhichPlayerBegin())
            {
                case FieldType.Cross:
                    Users[0].Type = FieldType.Cross;
                    Users[1].Type = FieldType.Circle;
                    break;
                case FieldType.Circle:
                    Users[0].Type = FieldType.Circle;
                    Users[1].Type = FieldType.Cross;
                    break;
                case FieldType.None:
                    throw new DomainException(DomainError.InvalidFieldType);
                default:
                    break;
            }
            return (Users.First(), Users.Last());
        }

        public void NewGame()
        {
            Chat = new();
            TicTacToeMatch = new();
            TicTacToeMatch.Intrepret(new ClearBoardEvent());
            AllowNewRound.Clear();
        }

        public void AddMessage(string author, string content)
            => Chat.Add(author, content);

        public ChatMessage GetLastMessage()
            => Chat.Last();

        public FieldType WhichPlayerBegin()
            => TicTacToeMatch.State switch
            {
                MatchState.CircleTurn => FieldType.Circle,
                MatchState.CrossTurn => FieldType.Cross,
                _ => FieldType.None
            };

        public char FieldTypeToChar(FieldType fieldType)
            => fieldType switch
            {
                FieldType.Circle => 'O',
                FieldType.Cross => 'X',
                _ => ' '
            };

        public char[][] GetBoardAsCharacters()
            => TicTacToeMatch.Board.Select(row => row.Select(field => FieldTypeToChar(field)).ToArray()).ToArray();

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
