using System.Text.Json.Serialization;
using TicTacToe.domain.Model.Common;
using TicTacToe.domain.Model.TicTacToe;

namespace TicTacToe.domain.Model.DataAccess
{
    public class PlayerView : BaseEntity
    {
        [JsonIgnore]
        public Guid GameViewId { get; set; }

        [JsonIgnore]
        public GameView GameView { get; set; }

        public Guid CognitoId { get; set; }

        public string ConnectionId { get; set; }

        public bool IsLoggedUser { get; set; } = false;

        public bool IsReady { get; set; } = false;

        public string Name { get; set; }

        public FieldType Type { get; set; } = FieldType.None;
    }
}
