using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SportEvents.Domain.Repositories;
using SportEvents.Web.Controllers;
using static SportEvents.Web.Constants;

namespace SportEvents.Test.Controllers
{
    public class HomeControllerTest
    {
        private readonly Mock<ITokenProvider> _tokenProviderMock;
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly HomeController _controller;

        public HomeControllerTest()
        {
            _tokenProviderMock = new Mock<ITokenProvider>();
            _loggerMock = new Mock<ILogger<HomeController>>();

            _controller = new HomeController(_loggerMock.Object, _tokenProviderMock.Object);
        }

        [Fact]
        public void Index_WhenUserIsNotAuthenticated_RedirectsToLogin()
        {
            // Arrange
            _tokenProviderMock.Setup(x => x.IsAuthenticated()).Returns(false);

            // Act
            var result = _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("Auth", redirectResult.ControllerName);

            // Verify logging
            _loggerMock.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == LogMessages.TokenExpiresMessage),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Index_WhenUserIsAuthenticated_ReturnsView()
        {
            // Arrange
            _tokenProviderMock.Setup(x => x.IsAuthenticated()).Returns(true);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
