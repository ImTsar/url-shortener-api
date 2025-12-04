using URLShortener.DTOs;
using URLShortener.InputModels;

namespace URLShortener.Interfaces
{
    public interface IShortUrlService
    {
        Task<int> CreateAsync(int userId, CreateShortUrlRequest model);
        Task<List<ShortUrlTableDto>> GetAllUrlsAsync();
        Task<ShortUrlDetailsDto> GetByIdAsync(int urlId);
        Task<string?> GetByShortCodeAsync(string code);
        Task DeleteAsync(int userId, int urlId);
    }
}
