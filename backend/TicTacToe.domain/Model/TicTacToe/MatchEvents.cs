namespace TicTacToe.domain.Model.TicTacToe
{
    public class ClearBoardEvent : IMatchEvent
    {
        public DateTime When { get; } = DateTime.UtcNow;

        public override string ToString()
            => $"{nameof(ClearBoardEvent)}";
    }

    public class MoveEvent : IMatchEvent
    {
        public DateTime When { get; } = DateTime.UtcNow;

        public FieldType WhoseTurn { get; }

        public (uint x, uint y) Target { get; }

        public MoveEvent((uint x, uint y) target, FieldType whoseTurn)
        {
            Target = target;
            WhoseTurn = whoseTurn;
        }

        public override string ToString()
            => $"{nameof(MoveEvent)}: move by {WhoseTurn} on ({Target.x},{Target.y})";
    }

    public class CheckWonEvent : IMatchEvent
    {
        public DateTime When { get; } = DateTime.UtcNow;

        public FieldType For { get; }

        public CheckWonEvent(FieldType @for)
        {
            For = @for;
        }

        public override string ToString()
            => $"{nameof(CheckWonEvent)}: Check for {For}";
    }

    public class CheckIfDrawEvent : IMatchEvent
    {
        public DateTime When { get; } = DateTime.UtcNow;
    }
}
