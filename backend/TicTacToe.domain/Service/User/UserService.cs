using Microsoft.EntityFrameworkCore;
using Serilog;
using TicTacToe.domain.Data;
using TicTacToe.domain.Model.DataAccess;
using TicTacToe.domain.Service.Auth;
using TicTacToe.domain.Service.GameHistory;
using TicTacToe.domain.Service.User.Responses;

namespace TicTacToe.domain.Service.User
{
    public class UserService : IUserService
    {
        private readonly IAuthService _authService;
        private readonly IGameHistoryService _gameHistoryService;
        private readonly ILogger _logger = Log.ForContext<UserService>();
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UserService(IAuthService authService, IGameHistoryService gameHistoryService, IDbContextFactory<AppDbContext> contextFactory)
        {
            _authService = authService;
            _gameHistoryService = gameHistoryService;
            _contextFactory = contextFactory;
        }

        // TODO refactor me, maybe some handle wrapper for all services?
        public async Task<IReadOnlyCollection<GameView>> GetAllGamesForAuthenticatedUser(string accessToken)
        {
            try
            {
                var externalUserData = await _authService.GetMeAsync(accessToken);

                if (externalUserData is null) throw new DomainException(DomainError.UserNotFound);
                if (!externalUserData.Attributes.TryGetValue("sub", out var cognitoSub)) throw new DomainException(DomainError.UserInfoNotFound);
                if (!Guid.TryParse(cognitoSub, out var cognitoId)) throw new DomainException(DomainError.ConvertError);

                using var context = await _contextFactory.CreateDbContextAsync();
                var userData = await context.Players.FirstOrDefaultAsync(p => p.CognitoId == cognitoId);

                if (userData is null) throw new DomainException(DomainError.UserInfoNotFound);

                return await context.Games
                    .Include(g => g.Users)
                    .Where(g => g.Users.Any(u => u.Id == userData.Id || u.Name.Equals(externalUserData.Username, StringComparison.OrdinalIgnoreCase)))
                    .ToListAsync();
            }
            catch (DomainException e)
            {
                _logger.Warning(e.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return null; // TODO return Failure
            }
        }

        public async Task<UserInfoResponse> GetUserInfo(string accessToken)
        {
            try
            {
                var externalUserData = await _authService.GetMeAsync(accessToken);

                if (externalUserData is null) throw new DomainException(DomainError.UserNotFound);
                if (!externalUserData.Attributes.TryGetValue("sub", out var cognitoSub)) throw new DomainException(DomainError.UserInfoNotFound);
                if (!Guid.TryParse(cognitoSub, out var cognitoId)) throw new DomainException(DomainError.ConvertError);

                using var context = await _contextFactory.CreateDbContextAsync();
                var userData = await context.Players.FirstOrDefaultAsync(p => p.CognitoId == cognitoId);

                if (userData is null) throw new DomainException(DomainError.UserInfoNotFound);

                var playerWins = await context.Games
                    .Where(g => g.WinnerId == userData.Id || g.WinnerName.Equals(externalUserData.Username, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync();

                return new UserInfoResponse(userData.Id, cognitoId, externalUserData.Username, playerWins is null ? 0 : playerWins.Count);
            }
            catch (DomainException e)
            {
                _logger.Warning(e.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return null; // TODO return Failure
            }
        }
    }
}
