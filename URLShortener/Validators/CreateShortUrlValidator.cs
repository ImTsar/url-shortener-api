using FluentValidation;
using URLShortener.InputModels;

namespace URLShortener.Validators
{
    public class CreateShortUrlValidator : AbstractValidator<CreateShortUrlRequest>
    {
        public CreateShortUrlValidator()
        {
            RuleFor(x => x.OriginalUrl)
                .NotEmpty().WithMessage("URL cannot be empty.")
                .MaximumLength(1024).WithMessage("URL is too long.")
                .Must(BeAValidUrl).WithMessage("Invalid URL format.");
        }

        private static bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
