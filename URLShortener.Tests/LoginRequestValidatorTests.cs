using FluentValidation.TestHelper;
using URLShortener.InputModels;
using URLShortener.Validators;

namespace URLShortener.Tests
{
    public class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator;

        public LoginRequestValidatorTests()
        {
            _validator = new LoginRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Username_Is_Empty()
        {
            var model = new LoginRequestModel
            {
                Username = "",
                Password = "pass123"
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Username)
                  .WithErrorMessage("Username is required");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var model = new LoginRequestModel
            {
                Username = "user123",
                Password = ""
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password is required");
        }

        [Fact]
        public void Should_Have_Errors_When_Both_Fields_Empty()
        {
            var model = new LoginRequestModel
            {
                Username = "",
                Password = ""
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Username);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Model_Is_Valid()
        {
            var model = new LoginRequestModel
            {
                Username = "user123",
                Password = "strongpassword"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Username);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}
