using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.InputModels;
using URLShortener.Interfaces;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutController(IAboutService _aboutService) : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _aboutService.GetContentAsync();

            return Ok(new { content });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAboutRequest request)
        {
            await _aboutService.UpdateContentAsync(request.Content);

            return NoContent();
        }
    }
}
