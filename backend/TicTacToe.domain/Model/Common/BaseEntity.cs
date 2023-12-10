using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TicTacToe.domain.Model.Common
{
    public class BaseEntity : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        [ConcurrencyCheck]
        public Guid Version { get; set; }

        public DateTime Modified { get; set; }

        public BaseEntity() => Modified = DateTime.Now;
    }
}
