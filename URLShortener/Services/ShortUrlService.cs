using AutoMapper;
using FluentValidation;
using URLShortener.DTOs;
using URLShortener.Enums;
using URLShortener.Exceptions;
using URLShortener.InputModels;
using URLShortener.Interfaces;
using URLShortener.Models;

namespace URLShortener.Services
{
    public class ShortUrlService(
        IShortUrlRepository _shortUrlRepository,
        IUserRepository _userRepository,
        IShortCodeGenerator _codeGenerator,
        IValidator<CreateShortUrlRequest> _createUrlValidator,
        IMapper _mapper
        ) : IShortUrlService
    {
        public async Task<List<ShortUrlTableDto>> GetAllUrlsAsync()
        {
            var urls = await _shortUrlRepository.GetAllAsync();

            return _mapper.Map<List<ShortUrlTableDto>>(urls);
        }

        public async Task<ShortUrlDetailsDto> GetByIdAsync(int urlId)
        {
            var url = await _shortUrlRepository.GetByIdAsync(urlId);

            if (url == null)
                throw new NotFoundException("URL not found.");

            return _mapper.Map<ShortUrlDetailsDto>(url);
        }

        public async Task<string?> GetByShortCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new CustomValidationException("Short code cannot be empty.");

            var url = await _shortUrlRepository.GetOriginalUrlByShortCodeAsync(code);

            if (url is null)
                throw new NotFoundException("Short URL not found.");

            return url;
        }

        public async Task<int> CreateAsync(int userId, CreateShortUrlRequest model)
        {

            var validationResult = await _createUrlValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
                throw new CustomValidationException(
                    string.Join("; ", validationResult.Errors.Select(x => x.ErrorMessage))
                );

            bool exists = await _shortUrlRepository.ExistsByOriginalAsync(model.OriginalUrl);

            if (exists)
                throw new ConflictException("This URL already exists.");

            string shortCode;
            do
            {
                shortCode = _codeGenerator.GenerateUniqueCode();
            }
            while (await _shortUrlRepository.CheckIfCodeExistsAsync(shortCode));

            var shortUrl = _mapper.Map<ShortUrl>(model);
            shortUrl.UserId = userId;
            shortUrl.ShortCode = shortCode;

            await _shortUrlRepository.AddAsync(shortUrl);

            return shortUrl.Id;
        }

        public async Task DeleteAsync(int userId, int urlId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found");

            bool isAdmin = user.Role == UserRole.Admin;

            var url = await _shortUrlRepository.GetByIdAsync(urlId);

            if (url == null)
                throw new NotFoundException("URL not found.");

            if (!isAdmin && url.UserId != userId)
                throw new ForbiddenException("You are not allowed to delete this URL.");

            await _shortUrlRepository.DeleteAsync(url);
        }
    }
}
