using URLShortener.Models;

namespace URLShortener.Interfaces
{
    public interface IAboutRepository
    {
        Task<AboutContent?> GetAsync();
        Task UpdateAsync(AboutContent content);
    }
}
