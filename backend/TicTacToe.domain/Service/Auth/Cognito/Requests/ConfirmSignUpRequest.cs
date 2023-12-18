namespace TicTacToe.domain.Service.Auth.Cognito.Requests
{
    public record ConfirmSignUpRequest(string Email, string ConfirmationCode);
}
