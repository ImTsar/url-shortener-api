using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.InputModels;
using URLShortener.Interfaces;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService _authService
        ) : Controller
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            var token = await _authService.LoginAsync(request);

            if (token is null)
                return Unauthorized();

            return Ok(new { token });
        }
    }
}
