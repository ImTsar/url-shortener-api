using FluentValidation.TestHelper;
using URLShortener.InputModels;
using URLShortener.Validators;

namespace URLShortener.Tests
{
    public class CreateShortUrlValidatorTests
    {
        private readonly CreateShortUrlValidator _validator;

        public CreateShortUrlValidatorTests()
        {
            _validator = new CreateShortUrlValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Url_Is_Empty()
        {
            var model = new CreateShortUrlRequest { OriginalUrl = "" };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.OriginalUrl)
                  .WithErrorMessage("URL cannot be empty.");
        }

        [Fact]
        public void Should_Have_Error_When_Url_Is_Too_Long()
        {
            var longUrl = "https://example.com/" + new string('a', 2000);

            var model = new CreateShortUrlRequest { OriginalUrl = longUrl };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.OriginalUrl)
                  .WithErrorMessage("URL is too long.");
        }

        [Theory]
        [InlineData("not-a-url")]
        [InlineData("http:/incorrect.com")]
        [InlineData("ftp://example.com")]
        [InlineData("www.google.com")]
        public void Should_Have_Error_When_Url_Format_Is_Invalid(string badUrl)
        {
            var model = new CreateShortUrlRequest { OriginalUrl = badUrl };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.OriginalUrl)
                  .WithErrorMessage("Invalid URL format.");
        }

        [Theory]
        [InlineData("http://google.com")]
        [InlineData("https://google.com")]
        [InlineData("https://sub.domain.example.com/path?x=1&y=2")]
        [InlineData("http://localhost:5000/test")]
        public void Should_Not_Have_Error_When_Url_Is_Valid(string validUrl)
        {
            var model = new CreateShortUrlRequest { OriginalUrl = validUrl };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.OriginalUrl);
        }

        [Fact]
        public void Should_Not_Have_Error_For_MaxLength_Exactly()
        {
            string url = "https://example.com/" + new string('a', 1000);

            var model = new CreateShortUrlRequest { OriginalUrl = url };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.OriginalUrl);
        }
    }
}
