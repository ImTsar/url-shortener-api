using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using URLShortener.Enums;
using URLShortener.Exceptions;
using URLShortener.InputModels;
using URLShortener.Interfaces;
using URLShortener.Models;
using URLShortener.Repositories;

namespace URLShortener.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IValidator<LoginRequestModel>> _validatorMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly IConfiguration _configuration;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _validatorMock = new Mock<IValidator<LoginRequestModel>>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();

            var config = new Dictionary<string, string>
            {
                { "Jwt:Key", "very_secret_jwt_key_123456789dfaafaadadfafdafadadfafdafdadfafdafd" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:ExpiresInMinutes", "30" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config!)
                .Build();

            _service = new AuthService(
                _userRepoMock.Object,
                _validatorMock.Object,
                _passwordHasherMock.Object,
                _configuration
            );
        }

        [Fact]
        public async Task LoginAsync_ThrowsValidationError_WhenModelIsInvalid()
        {
            var request = new LoginRequestModel { Username = "", Password = "" };

            _validatorMock.Setup(x => x.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("Username", "Username required")
                }));

            var action = async () => await _service.LoginAsync(request);

            await Assert.ThrowsAsync<CustomValidationException>(action);
        }

        [Fact]
        public async Task LoginAsync_Throws_WhenUserNotFound()
        {
            var request = new LoginRequestModel { Username = "test", Password = "123" };

            _validatorMock.Setup(x => x.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            _userRepoMock.Setup(r => r.GetByNameAsync("test"))
                .ReturnsAsync(null as User);

            var action = async () => await _service.LoginAsync(request);

            await Assert.ThrowsAsync<CustomValidationException>(action);
        }

        [Fact]
        public async Task LoginAsync_Throws_WhenPasswordIsInvalid()
        {
            var request = new LoginRequestModel { Username = "test", Password = "wrong" };

            _validatorMock.Setup(x => x.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            var user = new User
            {
                Id = 1,
                Username = "test",
                PasswordHash = "HASH"
            };

            _userRepoMock.Setup(x => x.GetByNameAsync("test"))
                .ReturnsAsync(user);

            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, "wrong"))
                .Returns(PasswordVerificationResult.Failed);

            var action = async () => await _service.LoginAsync(request);

            await Assert.ThrowsAsync<CustomValidationException>(action);
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenCredentialsValid()
        {
            var request = new LoginRequestModel { Username = "test", Password = "123" };

            _validatorMock.Setup(x => x.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            var user = new User
            {
                Id = 1,
                Username = "test",
                PasswordHash = "HASH",
                Role = UserRole.User
            };

            _userRepoMock.Setup(x => x.GetByNameAsync("test"))
                .ReturnsAsync(user);

            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user, "HASH", "123"))
                .Returns(PasswordVerificationResult.Success);

            var token = await _service.LoginAsync(request);

            Assert.NotNull(token);
            Assert.IsType<string>(token);

            Assert.Contains("ey", token);
        }
    }
}
