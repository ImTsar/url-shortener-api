using URLShortener.Services;

namespace URLShortener.Tests
{
    public class ShortCodeGeneratorTests
    {
        private readonly ShortCodeGenerator _generator = new();

        [Fact]
        public void GenerateUniqueCode_ReturnsCorrectLength()
        {
            int length = 6;

            var code = _generator.GenerateUniqueCode(length);

            Assert.Equal(length, code.Length);
        }

        [Fact]
        public void GenerateUniqueCode_ContainsOnlyAllowedCharacters()
        {
            const string allowed = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var code = _generator.GenerateUniqueCode();

            Assert.All(code, x => Assert.Contains(x, allowed));
        }

        [Fact]
        public void GenerateUniqueCode_ReturnsDifferentValues()
        {
            var code1 = _generator.GenerateUniqueCode();
            var code2 = _generator.GenerateUniqueCode();

            Assert.NotEqual(code1, code2);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(12)]
        public void GenerateUniqueCode_WorksWithDifferentLengths(int length)
        {
            var code = _generator.GenerateUniqueCode(length);

            Assert.Equal(length, code.Length);
        }
    }
}
