using Microsoft.EntityFrameworkCore;
using TicTacToe.domain.Data;
using TicTacToe.domain.Model.DataAccess;

namespace TicTacToe.domain.Service.GameHistory
{
    public class GameHistoryService : IGameHistoryService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GameHistoryService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IReadOnlyCollection<GameView>> GetAllGames()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Games
                .Include(g => g.MatchView)
                .Include(g => g.Users)
                .Include(g => g.MatchView.Messages)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<GameView>> GetAllGamesForUser(Guid userId)
        {
            if (userId == Guid.Empty) return new List<GameView>();

            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Games
                .Include(g => g.MatchView)
                .Include(g => g.Users)
                .Include(g => g.MatchView.Messages)
                .Where(g => g.Users.Any(u => u.Id == userId))
                .ToListAsync();
        }

        public async Task<GameView?> GetGame(Guid id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Games
                .Include(g => g.MatchView)
                .Include(g => g.Users)
                .Include(g => g.MatchView.Messages)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<bool> StoreGame(GameView game)
        {
            if (game is null) throw new ArgumentNullException("Null game cannot be stored in db");

            using var context = await _contextFactory.CreateDbContextAsync();
            context.Add(game);

            var numberOfNewEntries = await context.SaveChangesAsync();
            return numberOfNewEntries > 0 ? true : false;
        }
    }
}
