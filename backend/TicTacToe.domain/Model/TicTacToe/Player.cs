using TicTacToe.domain.Model.Common;

namespace TicTacToe.domain.Model.TicTacToe
{
    public class Player : BaseEntity
    {
        public Guid UserId { get; set; }

        public string ConnectionId { get; set; }

        public bool IsLoggedUser { get; } = false;

        public bool IsReady { get; set; } = false;

        public string Name { get; }

        public FieldType Type { get; set; } = FieldType.None;

        public Player(string connectionId)
        {
            Name = $"Not logged user - {connectionId}";
            ConnectionId = connectionId;
            UserId = Guid.Empty;
        }

        public Player(string connectionId, string name, Guid userId)
        {
            Name = name;
            ConnectionId = connectionId;
            UserId = userId;
            IsLoggedUser = true;
        }

        public bool IsPlayerTurn(MatchState state)
            => (state, Type) switch
            {
                (MatchState.CrossTurn, FieldType.Cross) => true,
                (MatchState.CircleTurn, FieldType.Circle) => true,
                _ => false
            };
    }
}
