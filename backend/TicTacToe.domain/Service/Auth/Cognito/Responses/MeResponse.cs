namespace TicTacToe.domain.Service.Auth.Cognito.Responses
{
    public record MeResponse(string Username, Dictionary<string, string> Attributes);
}
