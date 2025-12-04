namespace URLShortener.Interfaces
{
    public interface IShortCodeGenerator
    {
        string GenerateUniqueCode(int length = 6);
    }
}
