using URLShortener.Exceptions;
using URLShortener.Interfaces;

namespace URLShortener.Services
{
    public class AboutService(IAboutRepository _aboutRepository) : IAboutService
    {
        public async Task<string> GetContentAsync()
        {
            var about = await _aboutRepository.GetAsync();

            if (about == null)
                throw new NotFoundException("About content not found.");

            return about.Content;
        }

        public async Task UpdateContentAsync(string content)
        {
            var about = await _aboutRepository.GetAsync();

            if (about == null)
                throw new NotFoundException("About content not found.");

            about.Content = content;

            await _aboutRepository.UpdateAsync(about);
        }
    }
}
