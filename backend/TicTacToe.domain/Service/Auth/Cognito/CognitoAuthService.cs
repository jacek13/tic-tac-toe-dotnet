using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Security.Cryptography;
using System.Text;
using TicTacToe.domain.Service.Auth.Cognito.Requests;
using TicTacToe.domain.Service.Auth.Cognito.Responses;

namespace TicTacToe.domain.Service.Auth.Cognito
{
    public class CognitoAuthService : IAuthService
    {
        private readonly IConfigurationSection _awsConfig;
        private readonly IAmazonCognitoIdentityProvider _cognitoClient;
        private readonly ILogger _logger = Log.ForContext<CognitoAuthService>();

        public CognitoAuthService(IConfiguration awsConfig, IAmazonCognitoIdentityProvider provider)
        {
            _awsConfig = awsConfig.GetSection("AWS");
            _cognitoClient = provider;
        }

        public async Task<MeResponse> GetMeAsync(string? accessToken)
        {
            var cognitoRequest = new GetUserRequest
            {
                AccessToken = accessToken
            };

            var cognitoResponse = await _cognitoClient.GetUserAsync(cognitoRequest);

            var attributes = new Dictionary<string, string>();
            cognitoResponse.UserAttributes.ForEach(a => attributes.Add(a.Name, a.Value));

            return new MeResponse(cognitoResponse.Username, attributes);
        }

        public async Task<SignInResponse> SignInAsync(SignInRequest request)
        {
            var cognitoRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = _awsConfig.GetValue<string>("UserPoolClientId"),
            };

            cognitoRequest.AuthParameters.Add("USERNAME", request.Email);
            cognitoRequest.AuthParameters.Add("PASSWORD", request.Password);
            cognitoRequest.AuthParameters.Add("SECRET_HASH", CalculateSecretHash(request.Email, _awsConfig.GetValue<string>("UserPoolClientId"), _awsConfig.GetValue<string>("UserPoolClientSecret")));

            var cognitoResponse = await _cognitoClient.InitiateAuthAsync(cognitoRequest);
            var cognitoAuthResult = cognitoResponse.AuthenticationResult;

            _logger.Information("AWS responded with code: {code} for user: {email}", cognitoResponse.HttpStatusCode, request.Email);

            return new SignInResponse(cognitoAuthResult.IdToken, cognitoAuthResult.AccessToken, cognitoAuthResult.ExpiresIn, cognitoAuthResult.RefreshToken);
        }

        public async Task SignOutAsync(string? accessToken)
        {
            var cognitoRequest = new GlobalSignOutRequest
            {
                AccessToken = accessToken
            };

            await _cognitoClient.GlobalSignOutAsync(cognitoRequest);

            _logger.Information("User sing out");
        }

        public async Task<Responses.SignUpResponse> SignUpAsync(Requests.SignUpRequest request)
        {
            _logger.Information("Sign up request received. email: {email}", request.Email);

            var cognitoRequest = new Amazon.CognitoIdentityProvider.Model.SignUpRequest
            {
                ClientId = _awsConfig.GetValue<string>("UserPoolClientId"),
                Username = request.Email,
                Password = request.Password,
                SecretHash = CalculateSecretHash(request.Email, _awsConfig.GetValue<string>("UserPoolClientId"), _awsConfig.GetValue<string>("UserPoolClientSecret"))
            };

            cognitoRequest.UserAttributes.Add(new AttributeType { Name = "email", Value = request.Email });
            var cognitoResponse = await _cognitoClient.SignUpAsync(cognitoRequest);

            _logger.Information("AWS responded with code: {statudCode} for user email: {email}", cognitoResponse.HttpStatusCode, request.Email);

            return new Responses.SignUpResponse(
                cognitoResponse.UserSub,
                request.Email,
                $"A Confirmation Code has been sent to {cognitoResponse.CodeDeliveryDetails.Destination} via {cognitoResponse.CodeDeliveryDetails.DeliveryMedium.Value}");
        }

        private static string CalculateSecretHash(string username, string clientId, string clientSecret)
        {
            var secretKey = Encoding.UTF8.GetBytes(clientSecret);
            var message = Encoding.UTF8.GetBytes(username + clientId);

            using var hmac = new HMACSHA256(secretKey);

            var hash = hmac.ComputeHash(message);
            return Convert.ToBase64String(hash);
        }

        public async Task<bool> ConfirmSignUpAsync(Requests.ConfirmSignUpRequest request)
        {
            _logger.Information("Confirm sign up request received. email: {email}, confirmationCode: {code}", request.Email, request.ConfirmationCode);

            var cognitoRequest = new Amazon.CognitoIdentityProvider.Model.ConfirmSignUpRequest
            {
                ClientId = _awsConfig.GetValue<string>("UserPoolClientId"),
                Username = request.Email,
                ConfirmationCode = request.ConfirmationCode,
                SecretHash = CalculateSecretHash(request.Email, _awsConfig.GetValue<string>("UserPoolClientId"), _awsConfig.GetValue<string>("UserPoolClientSecret"))
            };
            var result = await _cognitoClient.ConfirmSignUpAsync(cognitoRequest);

            _logger.Information("AWS respond with code: {statusCode} for username: {email}", result.HttpStatusCode, request.Email);

            return true;
        }
    }
}
