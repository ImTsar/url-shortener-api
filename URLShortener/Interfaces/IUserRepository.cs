using URLShortener.Models;

namespace URLShortener.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByNameAsync(string name);
        Task<User?> GetByIdAsync(int id);
    }
}
