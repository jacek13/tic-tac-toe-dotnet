namespace TicTacToe.domain.Service.User.Responses
{
    public record UserInfoResponse(Guid Id, Guid CognitoId, string Name, int NumberOfWins);
}
