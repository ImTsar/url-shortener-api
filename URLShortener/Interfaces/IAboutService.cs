namespace URLShortener.Interfaces
{
    public interface IAboutService
    {
        Task<string> GetContentAsync();
        Task UpdateContentAsync(string content);
    }
}
