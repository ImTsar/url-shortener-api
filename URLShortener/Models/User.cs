using URLShortener.Enums;

namespace URLShortener.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; } = UserRole.User;
        public List<ShortUrl> ShortUrls { get; set; }
    }
}
