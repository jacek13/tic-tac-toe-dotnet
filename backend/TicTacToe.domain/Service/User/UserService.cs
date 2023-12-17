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
                    .Include(g => g.MatchView)
                    .Where(g => g.Users.Any(u => u.Id == userData.Id || u.Name == externalUserData.Username))
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

        public async Task<IReadOnlyCollection<UserGameHistoryStats>> GetAllGamesForAuthenticatedUsers()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var scoreBoard = await context.Games
                .Include(g => g.Users)
                .Where(g => g.WinnerId != Guid.Empty)
                .Select(g => new { g.WinnerId, g.WinnerName })
                .GroupBy(g => g.WinnerId)
                .Select(g => new UserGameHistoryStats(g.Key, g.ToList().FirstOrDefault().WinnerName, g.Count()))
                .ToListAsync();

            return scoreBoard;
        }

        public async Task<UserInfoResponse> GetUserInfo(string accessToken)
        {
            try
            {
                var externalUserData = await _authService.GetMeAsync(accessToken);

                if (externalUserData is null) throw new DomainException(DomainError.UserNotFound);
                if (!externalUserData.Attributes.TryGetValue("sub", out var cognitoSub)) throw new DomainException(DomainError.UserInfoNotFound);
                if (!externalUserData.Attributes.TryGetValue("email", out var cognitoEmail)) throw new DomainException(DomainError.UserInfoNotFound);
                if (!Guid.TryParse(cognitoSub, out var cognitoId)) throw new DomainException(DomainError.ConvertError);

                using var context = await _contextFactory.CreateDbContextAsync();
                var userData = await context.Players.FirstOrDefaultAsync(p => p.CognitoId == cognitoId);

                if (userData is null)
                {
                    _logger.Information("User: {cognitoId} data not found in db", cognitoId);
                    return new UserInfoResponse(cognitoId, cognitoId, cognitoEmail, 0);
                }

                var playerWins = await context.Games
                    .Where(g => g.WinnerId == userData.CognitoId || g.WinnerName == externalUserData.Username)
                    .ToListAsync();

                return new UserInfoResponse(userData.Id, cognitoId, cognitoEmail, playerWins is null ? 0 : playerWins.Count);
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
