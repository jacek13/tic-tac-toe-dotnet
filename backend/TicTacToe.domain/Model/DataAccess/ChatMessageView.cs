using System.Text.Json.Serialization;
using TicTacToe.domain.Model.Common;

namespace TicTacToe.domain.Model.DataAccess
{
    public class ChatMessageView : BaseEntity
    {
        [JsonIgnore]
        public Guid MatchViewId { get; set; }

        [JsonIgnore]
        public MatchView MatchView { get; set; }

        public DateTime When { get; set; }

        public string Content { get; set; }

        public string Author { get; set; }
    }
}
