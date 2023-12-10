using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TicTacToe.domain.Model.DataAccess;
using TicTacToe.domain.Model.TicTacToe;

namespace TicTacToe.domain.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<GameView> Games { get; set; }

        public DbSet<PlayerView> Players { get; set; }

        public DbSet<MatchView> Matches { get; set; }

        public DbSet<ChatMessageView> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("tictactoe");

            modelBuilder.Entity<PlayerView>()
                .HasOne(p => p.GameView)
                .WithMany(g => g.Users)
                .HasForeignKey(p => p.GameViewId);

            modelBuilder.Entity<GameView>()
                .HasOne(g => g.MatchView)
                .WithOne(m => m.GameView)
                .HasForeignKey<GameView>(g => g.MatchViewId);

            modelBuilder.Entity<MatchView>()
                .Property(m => m.Board)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                    }),
                    v => JsonSerializer.Deserialize<FieldType[][]>(v, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                    }));

            modelBuilder.Entity<MatchView>()
                .HasMany(m => m.Messages)
                .WithOne(cm => cm.MatchView)
                .HasForeignKey(m => m.MatchViewId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
