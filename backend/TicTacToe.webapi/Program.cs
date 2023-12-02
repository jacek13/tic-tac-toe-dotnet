using Serilog;
using TicTacToe.domain.Service;

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
            builder.Host.UseSerilog();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(c => c
                //.WithOrigins("*") // TODO Set origins
                //.SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .WithMethods("POST", "GET", "PUT", "DELETE", "OPTIONS")
                .AllowCredentials()
                .Build());

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseAuthorization();

            app.MapControllers();
            // TODO app.MapHub<>

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
