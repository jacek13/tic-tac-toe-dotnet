using TicTacToe.domain.Model.Common;

namespace TicTacToe.domain.Model.TicTacToe
{
    public class Match : BaseEntity, IMatchLogic<Match>
    {
        public MatchState State { get; private set; }

        public FieldType[][] Board { get; private set; }

        private static readonly uint BOARD_MAX_SIZE = 3;

        public Match()
        {
            State = MatchState.Warmup;
            Board = new FieldType[BOARD_MAX_SIZE][];

            for (int i = 0; i < BOARD_MAX_SIZE; i++)
            {
                Board[i] = new FieldType[BOARD_MAX_SIZE];
            }
        }

        public Match Intrepret(IMatchEvent @event)
            => this.Handle((dynamic)@event);

        public Match Handle(ClearBoardEvent @event)
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));
            if (State == MatchState.CircleWon || State == MatchState.CrossWon || State == MatchState.Draw) throw new DomainException(DomainError.MatchEnded);

            for (short i = 0; i < BOARD_MAX_SIZE; i++)
            {
                for (short j = 0; j < BOARD_MAX_SIZE; j++)
                {
                    Board[i][j] = FieldType.None;
                }
            }

            State = new Random().Next(0, 100) <= 50
                ? MatchState.CircleTurn
                : MatchState.CrossTurn;

            return this;
        }

        public Match Handle(MoveEvent @event)
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));
            if (State == MatchState.Draw || State == MatchState.CrossWon || State == MatchState.CircleWon) throw new DomainException(DomainError.MatchEnded);
            if (@event.WhoseTurn != FieldType.Cross && @event.WhoseTurn != FieldType.Circle) throw new DomainException(DomainError.InvalidMove);
            if (@event.WhoseTurn != (State == MatchState.CircleTurn ? FieldType.Circle : FieldType.Cross)) throw new DomainException(DomainError.InvalidMove);
            if (@event.Target.x >= BOARD_MAX_SIZE || @event.Target.y >= BOARD_MAX_SIZE) throw new DomainException(DomainError.InvalidCoordinates);
            if (Board[@event.Target.x][@event.Target.y] != FieldType.None) throw new DomainException(DomainError.InvalidMoveFieldAlreadyUsed);

            Board[@event.Target.x][@event.Target.y] = @event.WhoseTurn;
            State = @event.WhoseTurn == FieldType.Cross ? MatchState.CircleTurn : MatchState.CrossTurn;

            return this;
        }

        public Match Handle(CheckWonEvent @event)
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));
            if (@event.For != FieldType.Cross && @event.For != FieldType.Circle) throw new DomainException(DomainError.InvalidMove);

            for (int i = 0; i < BOARD_MAX_SIZE; i++)
            {
                if (Board[i][0] == @event.For && Board[i][1] == @event.For && Board[i][2] == @event.For)
                {
                    State = @event.For == FieldType.Cross ? MatchState.CrossWon : MatchState.CircleWon;
                    return this;
                }
            }

            for (int i = 0; i < BOARD_MAX_SIZE; i++)
            {
                if (Board[0][i] == @event.For && Board[1][i] == @event.For && Board[2][i] == @event.For)
                {
                    State = @event.For == FieldType.Cross ? MatchState.CrossWon : MatchState.CircleWon;
                    return this;
                }
            }

            if (Board[0][0] == @event.For && Board[1][1] == @event.For && Board[2][2] == @event.For)
            {
                State = @event.For == FieldType.Cross ? MatchState.CrossWon : MatchState.CircleWon;
                return this;
            }

            if (Board[0][2] == @event.For && Board[1][1] == @event.For && Board[2][0] == @event.For)
            {
                State = @event.For == FieldType.Cross ? MatchState.CrossWon : MatchState.CircleWon;
                return this;
            }

            return this;
        }

        public Match Handle(CheckIfDrawEvent @event)
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));

            var isDraw = !Board.Any(row => row.Any(field => field == FieldType.None));
            if (isDraw)
            {
                State = MatchState.Draw;
            }

            return this;
        }
    }
}
