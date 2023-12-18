using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.domain.Service.Auth;
using TicTacToe.domain.Service.Auth.Cognito.Requests;

namespace TicTacToe.webapi.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("auth/sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            try
            {
                return Ok(await _authService.SignUpAsync(request));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("auth/sign-up/confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUpConfirm([FromBody] ConfirmSignUpRequest request)
        {
            try
            {
                return Ok(await _authService.ConfirmSignUpAsync(request));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost("auth/sign-in")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            try
            {
                return Ok(await _authService.SignInAsync(request));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("auth/me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var accessToken = HttpContext?.Request?.Headers["Authorization"].ToString()?.Substring(7);
            return string.IsNullOrWhiteSpace(accessToken)
                ? BadRequest()
                : Ok(await _authService.GetMeAsync(accessToken));
        }

        [HttpPost("auth/sign-out")]
        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            var accessToken = HttpContext?.Request?.Headers["Authorization"].ToString()?.Substring(7);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return BadRequest();
            }

            await _authService.SignOutAsync(accessToken);
            return Ok();
        }
    }
}
