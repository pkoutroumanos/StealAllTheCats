using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StealAllTheCats.Controllers;
using StealAllTheCats.Dtos;
using StealAllTheCats.Services;
using Xunit;

namespace StealAllTheCats.Tests.Controllers
{
    public class CatsControllerTests
    {
        [Fact]
        public async Task GetCatById_ReturnsNotFound_WhenCatDoesNotExist()
        {
            // Arrange
            var mockCatQueryService = new Mock<ICatQueryService>();
            mockCatQueryService.Setup(s => s.GetCatByIdAsync(It.IsAny<int>()))
                               .ReturnsAsync((CatDto?)null);
            var mockCatApiService = new Mock<ICatApiService>();
            var mockLogger = new Mock<ILogger<CatsController>>();

            var controller = new CatsController(mockCatApiService.Object, mockCatQueryService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetCatById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCatById_ReturnsOk_WhenCatExists()
        {
            // Arrange
            var catDto = new CatDto { Id = 1, CatId = "cat123", Width = 300, Height = 200, Created = DateTime.UtcNow };
            var mockCatQueryService = new Mock<ICatQueryService>();
            mockCatQueryService.Setup(s => s.GetCatByIdAsync(1))
                               .ReturnsAsync(catDto);
            var mockCatApiService = new Mock<ICatApiService>();
            var mockLogger = new Mock<ILogger<CatsController>>();

            var controller = new CatsController(mockCatApiService.Object, mockCatQueryService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetCatById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CatDto>(okResult.Value);
            Assert.Equal("cat123", returnValue.CatId);
        }
    }
}
