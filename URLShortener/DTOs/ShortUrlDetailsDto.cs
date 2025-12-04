namespace URLShortener.DTOs
{
    public class ShortUrlDetailsDto
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = null!;
        public string ShortCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string OwnerName { get; set; } = null!;
    }
}
