namespace TicTacToe.domain.Service.User.Responses
{
    public record UserGameHistoryStats(Guid UserId, string Email, int Wins);
}
