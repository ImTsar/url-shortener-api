using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.Interfaces;
using URLShortener.Models;

namespace URLShortener.Repositories
{
    public class UserRepository(AppDbContext _dbContext) : IUserRepository
    {
        public async Task<User?> GetByNameAsync(string name)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Username == name);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
