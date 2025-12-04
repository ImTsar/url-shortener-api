namespace URLShortener.DTOs
{
    public class ShortUrlTableDto
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = null!;
        public string ShortCode { get; set; } = null!;
        public int UserId { get; set; }
    }
}
