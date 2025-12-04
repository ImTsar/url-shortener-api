using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Extensions;
using URLShortener.InputModels;
using URLShortener.Interfaces;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShortUrlController(IShortUrlService _shortUrlService) : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllUrls()
        {
            var list = await _shortUrlService.GetAllUrlsAsync();

            return Ok(list);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var url = await _shortUrlService.GetByIdAsync(id);

            return Ok(url);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateShortUrlRequest request)
        {
            var userId = User.GetUserId();

            int newUrlId = await _shortUrlService.CreateAsync(userId, request);

            return CreatedAtAction(nameof(GetById), new { id = newUrlId }, newUrlId);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = User.GetUserId();
            await _shortUrlService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}
