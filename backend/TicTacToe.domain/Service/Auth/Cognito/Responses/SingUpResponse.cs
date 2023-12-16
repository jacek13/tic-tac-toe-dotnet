namespace TicTacToe.domain.Service.Auth.Cognito.Responses
{
    public record SignUpResponse(string UserId, string Email, string Message);
}
