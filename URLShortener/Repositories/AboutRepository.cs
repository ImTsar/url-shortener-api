using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.Interfaces;
using URLShortener.Models;

namespace URLShortener.Repositories
{
    public class AboutRepository(AppDbContext _dbContext) : IAboutRepository
    {
        public async Task<AboutContent?> GetAsync()
        {
            return await _dbContext.AboutContents.FirstAsync();
        }

        public async Task UpdateAsync(AboutContent content)
        {
            _dbContext.AboutContents.Update(content);
            await _dbContext.SaveChangesAsync();
        }
    }
}
