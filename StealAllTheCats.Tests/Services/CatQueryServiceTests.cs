using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using StealAllTheCats.Dtos;
using StealAllTheCats.Mapping;
using StealAllTheCats.Models;
using StealAllTheCats.Repositories;
using StealAllTheCats.Services;
using Xunit;

namespace StealAllTheCats.Tests.Services
{
    public class CatQueryServiceTests
    {
        private readonly IMapper _mapper;

        public CatQueryServiceTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public async Task GetCatByIdAsync_ShouldReturnNull_WhenCatNotFound()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.Cats.GetByIdAsync(It.IsAny<int>()))
                          .ReturnsAsync((CatEntity?)null);
            var service = new CatQueryService(mockUnitOfWork.Object, _mapper);

            // Act
            var result = await service.GetCatByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCatByIdAsync_ShouldReturnCatDto_WhenCatExists()
        {
            // Arrange: create a sample CatEntity.
            var catEntity = new CatEntity
            {
                Id = 1,
                CatId = "cat123",
                Width = 300,
                Height = 200,
                Created = DateTime.UtcNow,
                CatTags = new List<CatTag>()
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.Cats.GetByIdAsync(1))
                          .ReturnsAsync(catEntity);
            var service = new CatQueryService(mockUnitOfWork.Object, _mapper);

            // Act
            var result = await service.GetCatByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("cat123", result!.CatId);
        }
    }
}