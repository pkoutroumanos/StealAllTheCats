using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StealAllTheCats.Data;
using StealAllTheCats.Models;
using StealAllTheCats.Repositories;
using Xunit;

namespace StealAllTheCats.Tests.Repositories
{
    public class CatRepositoryTests
    {
        // Helper method to create a new in-memory database context for each test.
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB for each test
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCatDoesNotExist()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var mockLogger = new Mock<ILogger<CatRepository>>();
            var repository = new CatRepository(context, mockLogger.Object);

            // Act
            var cat = await repository.GetByIdAsync(1);

            // Assert
            Assert.Null(cat);
        }

        [Fact]
        public async Task AddAsync_ThenGetByIdAsync_ShouldReturnCatEntity()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var mockLogger = new Mock<ILogger<CatRepository>>();
            var repository = new CatRepository(context, mockLogger.Object);

            var newCat = new CatEntity
            {
                CatId = "cat123",
                Width = 200,
                Height = 150,
                Image = new byte[] { 1, 2, 3 },
                Created = DateTime.UtcNow,
                CatTags = new System.Collections.Generic.List<CatTag>()
            };

            // Act
            await repository.AddAsync(newCat);
            await context.SaveChangesAsync(); // commit changes
            var catFromDb = await repository.GetByIdAsync(newCat.Id);

            // Assert
            Assert.NotNull(catFromDb);
            Assert.Equal("cat123", catFromDb.CatId);
        }
    }
}