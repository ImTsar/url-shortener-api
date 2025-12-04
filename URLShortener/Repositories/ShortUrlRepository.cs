using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.Interfaces;
using URLShortener.Models;

namespace URLShortener.Repositories
{
    public class ShortUrlRepository(AppDbContext _dbContext) : IShortUrlRepository
    {
        public async Task AddAsync(ShortUrl shortUrl)
        {
            await _dbContext.ShortUrls.AddAsync(shortUrl);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<string?> GetOriginalUrlByShortCodeAsync(string code)
        {
            return await _dbContext.ShortUrls
                .Where(s => s.ShortCode == code)
                .Select(s => s.OriginalUrl)
                .FirstOrDefaultAsync();
        }

        public async Task<ShortUrl?> GetByIdAsync(int id)
        {
            return await _dbContext.ShortUrls
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByOriginalAsync(string originalUrl)
        {
            return await _dbContext.ShortUrls
                .AnyAsync(x => x.OriginalUrl == originalUrl);
        }


        public async Task<List<ShortUrl>> GetAllAsync()
        {
            return await _dbContext.ShortUrls
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task DeleteAsync(ShortUrl url)
        {
            _dbContext.ShortUrls.Remove(url);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CheckIfCodeExistsAsync(string code)
        {
            return await _dbContext.ShortUrls.AnyAsync(x => x.ShortCode == code);
        }
    }
}
