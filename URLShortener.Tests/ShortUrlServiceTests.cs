using AutoMapper;
using FluentValidation;
using Moq;
using URLShortener.DTOs;
using URLShortener.Enums;
using URLShortener.Exceptions;
using URLShortener.InputModels;
using URLShortener.Interfaces;
using URLShortener.Models;
using URLShortener.Services;

namespace URLShortener.Tests
{
    public class ShortUrlServiceTests
    {
        private readonly Mock<IShortUrlRepository> _repoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IShortCodeGenerator> _codeGenMock;
        private readonly Mock<IValidator<CreateShortUrlRequest>> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly ShortUrlService _service;

        public ShortUrlServiceTests()
        {
            _repoMock = new Mock<IShortUrlRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _codeGenMock = new Mock<IShortCodeGenerator>();
            _validatorMock = new Mock<IValidator<CreateShortUrlRequest>>();
            _mapperMock = new Mock<IMapper>();

            _service = new ShortUrlService(
                _repoMock.Object,
                _userRepoMock.Object,
                _codeGenMock.Object,
                _validatorMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetAllUrlsAsync_ReturnsMappedDtos()
        {
            var urls = new List<ShortUrl>
            {
                new ShortUrl { Id = 1, OriginalUrl = "https://test.com" }
            };

            var dtos = new List<ShortUrlTableDto>
            {
                new ShortUrlTableDto { Id = 1, OriginalUrl = "https://test.com" }
            };

            _repoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(urls);
            _mapperMock.Setup(x => x.Map<List<ShortUrlTableDto>>(urls)).Returns(dtos);

            var result = await _service.GetAllUrlsAsync();

            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenExists()
        {
            var url = new ShortUrl { Id = 1, OriginalUrl = "https://test.com" };
            var dto = new ShortUrlDetailsDto { Id = 1, OriginalUrl = "https://test.com" };

            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(url);
            _mapperMock.Setup(x => x.Map<ShortUrlDetailsDto>(url)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFound_WhenNull()
        {
            _repoMock.Setup(x => x.GetByIdAsync(1))
                     .ReturnsAsync(null as ShortUrl);

            var action = async () => await _service.GetByIdAsync(1);

            await Assert.ThrowsAsync<NotFoundException>(action);
        }

        [Fact]
        public async Task GetByShortCodeAsync_ReturnsOriginalUrl()
        {
            _repoMock.Setup(x => x.GetOriginalUrlByShortCodeAsync("abc"))
                     .ReturnsAsync("https://google.com");

            var result = await _service.GetByShortCodeAsync("abc");

            Assert.Equal("https://google.com", result);
        }

        [Fact]
        public async Task GetByShortCodeAsync_ThrowsValidation_WhenEmpty()
        {
            var action = async () => await _service.GetByShortCodeAsync("");

            await Assert.ThrowsAsync<CustomValidationException>(action);
        }

        [Fact]
        public async Task GetByShortCodeAsync_ThrowsNotFound_WhenMissing()
        {
            _repoMock.Setup(x => x.GetOriginalUrlByShortCodeAsync("abc"))
                     .ReturnsAsync(null as string);

            var action = async () => await _service.GetByShortCodeAsync("abc");

            await Assert.ThrowsAsync<NotFoundException>(action);
        }

        [Fact]
        public async Task CreateAsync_ThrowsValidation_WhenModelInvalid()
        {
            var model = new CreateShortUrlRequest { OriginalUrl = "" };

            _validatorMock.Setup(x => x.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult
                {
                    Errors = { new FluentValidation.Results.ValidationFailure("OriginalUrl", "error") }
                });

            var action = async () => await _service.CreateAsync(1, model);

            await Assert.ThrowsAsync<CustomValidationException>(action);
        }

        [Fact]
        public async Task CreateAsync_ThrowsConflict_WhenUrlExists()
        {
            var model = new CreateShortUrlRequest { OriginalUrl = "https://test.com" };

            _validatorMock.Setup(x => x.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _repoMock.Setup(x => x.ExistsByOriginalAsync("https://test.com"))
                .ReturnsAsync(true);

            var action = async () => await _service.CreateAsync(1, model);

            await Assert.ThrowsAsync<ConflictException>(action);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNewId_WhenSuccess()
        {
            var model = new CreateShortUrlRequest { OriginalUrl = "https://test.com" };
            var mapped = new ShortUrl { Id = 5, OriginalUrl = "https://test.com" };

            _validatorMock.Setup(x => x.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _repoMock.Setup(x => x.ExistsByOriginalAsync(model.OriginalUrl)).ReturnsAsync(false);

            _codeGenMock.Setup(x => x.GenerateUniqueCode(6)).Returns("abc123");
            _repoMock.Setup(x => x.CheckIfCodeExistsAsync("abc123")).ReturnsAsync(false);

            _mapperMock.Setup(x => x.Map<ShortUrl>(model)).Returns(mapped);

            _repoMock.Setup(x => x.AddAsync(mapped))
                .Callback(() => mapped.Id = 99)
                .Returns(Task.CompletedTask);

            var id = await _service.CreateAsync(1, model);

            Assert.Equal(99, id);
            Assert.Equal("abc123", mapped.ShortCode);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFound_WhenUserMissing()
        {
            _userRepoMock.Setup(x => x.GetByIdAsync(1))
                         .ReturnsAsync(null as User);

            var action = async () => await _service.DeleteAsync(1, 10);

            await Assert.ThrowsAsync<NotFoundException>(action);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFound_WhenUrlMissing()
        {
            _userRepoMock.Setup(x => x.GetByIdAsync(1))
                         .ReturnsAsync(new User { Id = 1, Role = UserRole.User });

            _repoMock.Setup(x => x.GetByIdAsync(10))
                     .ReturnsAsync(null as ShortUrl);

            var action = async () => await _service.DeleteAsync(1, 10);

            await Assert.ThrowsAsync<NotFoundException>(action);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsForbidden_WhenNotOwnerAndNotAdmin()
        {
            _userRepoMock.Setup(x => x.GetByIdAsync(1))
                            .ReturnsAsync(new User { Id = 1, Role = UserRole.User });

            _repoMock.Setup(x => x.GetByIdAsync(22))
                .ReturnsAsync(new ShortUrl { Id = 22, UserId = 5 });

            var action = async () => await _service.DeleteAsync(1, 22);

            await Assert.ThrowsAsync<ForbiddenException>(action);
        }

        [Fact]
        public async Task DeleteAsync_AllowsAdminToDeleteAnyUrl()
        {
            _userRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new User { Id = 1, Role = UserRole.Admin });

            var url = new ShortUrl { Id = 1, UserId = 99 };
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(url);

            _repoMock.Setup(x => x.DeleteAsync(url)).Returns(Task.CompletedTask);

            var action = async () => await _service.DeleteAsync(1, 1);

            await action();
        }
    }
}
