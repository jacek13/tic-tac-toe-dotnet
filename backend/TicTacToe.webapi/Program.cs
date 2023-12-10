using Microsoft.EntityFrameworkCore;
using Serilog;
using TicTacToe.domain.Data;
using TicTacToe.domain.Service;
using TicTacToe.domain.Service.GameHistory;
using TicTacToe.webapi.Hubs;

namespace TicTacToe.webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IGameService, GameService>();
            builder.Services.AddSingleton<IGameHistoryService, GameHistoryService>();
            builder.Services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetValue<string>("db:connection"));
            });

            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("dockerlocal"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            using var dbContext = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext();
            dbContext.Database.EnsureCreated();

            app.UseCors(c => c
                //.WithOrigins("http://localhost:5000", "https://localhost:5001", "http://localhost:4200")
                //.SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyOrigin() // TODO remove later
                .AllowAnyHeader()
                .WithMethods("POST", "GET", "PUT", "DELETE", "OPTIONS")
                //.AllowCredentials()
                .Build());

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<GameHub>("/hub/game");

            Log.Information("Starting in environment: {environment}", app.Environment.EnvironmentName);
            try
            {
                app.Run();
            }
            catch (Exception e)
            {
                Log.Fatal("Error message: {message}", e.Message);
            }
            finally
            {
                Log.Information("Closing app...");
                Log.CloseAndFlush();
            }
        }
    }
}
