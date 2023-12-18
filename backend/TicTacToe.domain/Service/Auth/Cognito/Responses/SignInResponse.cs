namespace TicTacToe.domain.Service.Auth.Cognito.Responses
{
    public record SignInResponse(string IdToken, string Token, int ExpiresIn, string RefreshToken);
}
