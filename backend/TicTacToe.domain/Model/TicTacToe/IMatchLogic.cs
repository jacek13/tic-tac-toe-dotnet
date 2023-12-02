namespace TicTacToe.domain.Model.TicTacToe
{
    public interface IMatchLogic<T>
    {
        public MatchState State { get; }

        public FieldType[][] Board { get; }

        public T Intrepret(IMatchEvent @event);
    }

    public interface IMatchEvent
    {
        public DateTime When { get; }
    }
}
