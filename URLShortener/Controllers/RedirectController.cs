using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Interfaces;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("")]
    public class RedirectController(IShortUrlService _shortUrlService) : Controller
    {

        [HttpGet("{shortCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var url = await _shortUrlService.GetByShortCodeAsync(shortCode);

            return Redirect(url);
        }
    }
}
