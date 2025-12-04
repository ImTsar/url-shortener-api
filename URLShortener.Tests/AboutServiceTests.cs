using Moq;
using URLShortener.Exceptions;
using URLShortener.Interfaces;
using URLShortener.Models;
using URLShortener.Services;

namespace URLShortener.Tests
{
    public class AboutServiceTests
    {
        private readonly Mock<IAboutRepository> _repoMock;
        private readonly AboutService _service;

        public AboutServiceTests()
        {
            _repoMock = new Mock<IAboutRepository>();
            _service = new AboutService(_repoMock.Object);
        }

        [Fact]
        public async Task GetContentAsync_ReturnsContent_WhenAboutExists()
        {
            var about = new AboutContent { Id = 1, Content = "Test content" };

            _repoMock.Setup(x => x.GetAsync())
                     .ReturnsAsync(about);

            var result = await _service.GetContentAsync();

            Assert.Equal("Test content", result);
        }

        [Fact]
        public async Task GetContentAsync_ThrowsNotFound_WhenAboutIsNull()
        {
            _repoMock.Setup(x => x.GetAsync())
                     .ReturnsAsync(null as AboutContent);
            var action = async () => await _service.GetContentAsync();

            await Assert.ThrowsAsync<NotFoundException>(action);
        }

        [Fact]
        public async Task UpdateContentAsync_UpdatesContent_WhenAboutExists()
        {
            var about = new AboutContent { Id = 1, Content = "Old content" };

            _repoMock.Setup(x => x.GetAsync())
                     .ReturnsAsync(about);

            _repoMock.Setup(x => x.UpdateAsync(It.IsAny<AboutContent>()))
                     .Returns(Task.CompletedTask);

            await _service.UpdateContentAsync("New content");

            Assert.Equal("New content", about.Content);
        }

        [Fact]
        public async Task UpdateContentAsync_ThrowsNotFound_WhenAboutIsNull()
        {
            _repoMock.Setup(x => x.GetAsync())
                     .ReturnsAsync(null as AboutContent);

            var action = async () => await _service.UpdateContentAsync("New content");

            await Assert.ThrowsAsync<NotFoundException>(action);
        }
    }
}