namespace URLShortener.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = null!;
        public string ShortCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
