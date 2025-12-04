using URLShortener.Interfaces;

namespace URLShortener.Services
{
    public class ShortCodeGenerator : IShortCodeGenerator
    {
        private const string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly Random _random = new();

        public string GenerateUniqueCode(int length = 6)
        {
            return new string([.. Enumerable.Repeat(_chars, length).Select(s => s[_random.Next(s.Length)])]);
        }
    }
}
