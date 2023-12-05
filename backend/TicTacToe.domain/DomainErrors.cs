namespace TicTacToe.domain
{
    public enum DomainError
    {
        InvalidCrossMove,
        InvalidCircleMove,
        InvalidMove,
        InvalidCoordinates,
        MatchEnded,
        ValidationError
    }

    public class DomainException : Exception
    {
        public DomainError ErrorKind { get; }

        public DomainException(DomainError errorKind) : base(errorKind.ToString()) { }
    }
}
