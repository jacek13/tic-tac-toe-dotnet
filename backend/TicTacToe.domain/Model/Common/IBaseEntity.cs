namespace TicTacToe.domain.Model.Common
{
    public interface IBaseEntity
    {
        public Guid Id { get; }

        public Guid Version { get; set; }

        public DateTime Modified { get; set; }
    }
}
