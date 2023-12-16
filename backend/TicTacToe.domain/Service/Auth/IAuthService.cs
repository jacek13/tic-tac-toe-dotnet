using TicTacToe.domain.Service.Auth.Cognito.Requests;
using TicTacToe.domain.Service.Auth.Cognito.Responses;

namespace TicTacToe.domain.Service.Auth
{
    public interface IAuthService
    {
        Task<SignUpResponse> SignUpAsync(SignUpRequest request);

        Task<SignInResponse> SignInAsync(SignInRequest request);

        Task<MeResponse> GetMeAsync(string? accessToken);

        Task<bool> ConfirmSignUpAsync(ConfirmSignUpRequest request);

        Task SignOutAsync(string? accessToken);
    }
}
