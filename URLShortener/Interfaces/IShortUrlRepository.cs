using URLShortener.Models;

namespace URLShortener.Interfaces
{
    public interface IShortUrlRepository
    {
        Task<List<ShortUrl>> GetAllAsync();
        Task<string?> GetOriginalUrlByShortCodeAsync(string code);
        Task<ShortUrl?> GetByIdAsync(int id);
        Task<bool> ExistsByOriginalAsync(string originalUrl);
        Task<bool> CheckIfCodeExistsAsync(string code);
        Task AddAsync(ShortUrl shortUrl);
        Task DeleteAsync(ShortUrl url);
    }
}
