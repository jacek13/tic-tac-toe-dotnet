using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TicTacToe.domain.Data;
using TicTacToe.domain.Service;
using TicTacToe.domain.Service.Auth;
using TicTacToe.domain.Service.Auth.Cognito;
using TicTacToe.domain.Service.GameHistory;
using TicTacToe.domain.Service.User;
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

            // TODO Move to seperate file that configures services: public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IGameService, GameService>();
            builder.Services.AddSingleton<IGameHistoryService, GameHistoryService>();
            builder.Services.AddSingleton<IAuthService, CognitoAuthService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddCognitoIdentity();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetValue<string>("db:connection"));
            });
            builder.Services.AddSwaggerGen(c =>
            {
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = configuration.GetValue<string>("AWS:Authority");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false
                };
            });
            builder.Services.AddSingleton<IAmazonCognitoIdentityProvider>(provider =>
            {
                var region = configuration.GetValue<string>("AWS:Region");
                var accessKey = configuration.GetValue<string>("AWS:UserPoolClientId");
                var secretKey = configuration.GetValue<string>("AWS:UserPoolClientSecret");
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                var cognitoClient = new AmazonCognitoIdentityProviderClient(credentials, RegionEndpoint.GetBySystemName(region));

                return cognitoClient;
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
            app.UseAuthentication();
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
