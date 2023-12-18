namespace TicTacToe.domain
{
    public enum DomainError
    {
        InvalidCrossMove,
        InvalidCircleMove,
        InvalidMove,
        InvalidMoveFieldAlreadyUsed,
        InvalidCoordinates,
        InvalidPlayerCount,
        InvalidFieldType,
        MatchEnded,
        ValidationError,
        NotLoggedUser,
        UserNotFound,
        UserInfoNotFound,
        ConvertError,
        GameNotFound
    }

    public class DomainException : Exception
    {
        public DomainError ErrorKind { get; }

        public DomainException(DomainError errorKind) : base(errorKind.ToString()) { }
    }
}
