using System.Text.Json.Serialization;
using TicTacToe.domain.Model.Common;
using TicTacToe.domain.Model.TicTacToe;

namespace TicTacToe.domain.Model.DataAccess
{
    public class MatchView : BaseEntity
    {
        [JsonIgnore]
        public Guid GameViewId { get; set; }

        [JsonIgnore]
        public GameView GameView { get; set; }

        public ICollection<ChatMessageView> Messages { get; set; }

        public FieldType[][] Board { get; set; }

        public MatchState State { get; set; }
    }
}