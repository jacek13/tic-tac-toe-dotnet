using System.Text.Json.Serialization;
using TicTacToe.domain.Model.Common;

namespace TicTacToe.domain.Model.DataAccess
{
    public class GameView : BaseEntity
    {
        [JsonIgnore]
        public Guid MatchViewId { get; set; }

        public Guid WinnerId { get; set; }

        public string WinnerName { get; set; }

        public MatchView MatchView { get; set; }

        public ICollection<PlayerView> Users { get; set; }
    }
}
