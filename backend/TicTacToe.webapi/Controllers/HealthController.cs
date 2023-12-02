using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.webapi.Controllers
{

    [ApiController]
    public class HealthController : ControllerBase
    {
        public HealthController() { }

        [HttpGet("/health")]
        public object GetInfo()
        {
            return new
            {
                EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                ServerDate = DateTime.Now,
            };
        }

    }
}
