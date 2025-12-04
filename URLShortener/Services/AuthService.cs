using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using URLShortener.Exceptions;
using URLShortener.InputModels;
using URLShortener.Interfaces;
using URLShortener.Models;

namespace URLShortener.Repositories
{
    public class AuthService(
        IUserRepository _userRepository,
        IValidator<LoginRequestModel> _loginValidator,
        IPasswordHasher<User> _passwordHasher,
        IConfiguration _config

        ) : IAuthService
    {
        public async Task<string?> LoginAsync(LoginRequestModel request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
                throw new CustomValidationException(
                    string.Join("; ", validationResult.Errors.Select(x => x.ErrorMessage))
                );

            var user = await _userRepository.GetByNameAsync(request.Username);
            if (user == null)
                throw new CustomValidationException("Invalid username or password");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new CustomValidationException("Invalid username or password");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("role", user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            try
            {
                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"])),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JWT ERROR: {ex.Message}");
                throw;
            }
        }
    }
}
