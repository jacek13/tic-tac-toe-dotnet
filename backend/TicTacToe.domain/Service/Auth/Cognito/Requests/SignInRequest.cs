namespace TicTacToe.domain.Service.Auth.Cognito.Requests
{
    public record SignInRequest(string Email, string Password);
}
