using URLShortener.InputModels;

namespace URLShortener.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginRequestModel request);
    }
}
