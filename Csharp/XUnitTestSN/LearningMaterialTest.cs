using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using IClients;
using IServices;
using Models;
using Models.DTOs;
using Microsoft.Extensions.Logging;
using Services;

namespace XUnitTestSN
{
    public class LearningMaterialTest
    {
        private readonly Mock<ILearningMaterialClient> _learningMaterialClientMock;
        private readonly Mock<ITokenHelper> _tokenHelperMock;
        private readonly Mock<ILogger<LearningMaterialService>> _loggerMock;
        private readonly ILearningMaterialService _learningMaterialService;

        public LearningMaterialTest()
        {
            _learningMaterialClientMock = new Mock<ILearningMaterialClient>();
            _tokenHelperMock = new Mock<ITokenHelper>();
            _loggerMock = new Mock<ILogger<LearningMaterialService>>();
            _learningMaterialService = new LearningMaterialService(_learningMaterialClientMock.Object, _tokenHelperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddLearningMaterialAsync_ShouldReturnLearningMaterial_WhenCreationIsSuccessful()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1L;
            var createLearningMaterialDto = new CreateLearningMaterial
            {
                Headline = "Test Headline",
                Description = "Test Description",
                Contents = new List<Content> { new TextContent { Body = "Test Content" } }
            };
            var expectedLearningMaterial = new LearningMaterial { Headline = createLearningMaterialDto.Headline };

            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(userId);
            _learningMaterialClientMock.Setup(client => client.AddLearningMaterialAsync(It.IsAny<CreateLearningMaterial>())).ReturnsAsync(expectedLearningMaterial);

            // Act
            var result = await _learningMaterialService.AddLearningMaterialAsync(createLearningMaterialDto, token);

            // Assert
            Assert.Equal(expectedLearningMaterial, result);
        }

        [Fact]
        public async Task AddLearningMaterialAsync_ShouldThrowArgumentException_WhenContentsAreEmpty()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1L;
            var createLearningMaterialDto = new CreateLearningMaterial
            {
                Headline = "Test Headline",
                Description = "Test Description",
                Contents = new List<Content>()
            };

            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(userId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _learningMaterialService.AddLearningMaterialAsync(createLearningMaterialDto, token));
            Assert.Equal("Learning material cannot be created without at least one content.", exception.Message);
        }

        [Fact]
        public async Task AddLearningMaterialAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var token = "invalid_token";
            var createLearningMaterialDto = new CreateLearningMaterial
            {
                Headline = "Test Headline",
                Description = "Test Description",
                Contents = new List<Content> { new TextContent { Body = "Test Content" } }
            };

            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(0);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _learningMaterialService.AddLearningMaterialAsync(createLearningMaterialDto, token));
            Assert.Equal("Only registered users can create content.", exception.Message);
        }

        [Fact]
        public async Task DeleteLearningMaterialAsync_ShouldComplete_WhenUserIsAuthorized()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1L;
            var learningMaterialId = 1L;
            var existingLearningMaterial = new LearningMaterial { Id = learningMaterialId, UserId = userId };

            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(userId);
            _learningMaterialClientMock.Setup(client => client.GetLearningMaterialByIdAsync(learningMaterialId)).ReturnsAsync(existingLearningMaterial);
            _learningMaterialClientMock.Setup(client => client.DeleteLearningMaterialAsync(learningMaterialId)).Returns(Task.CompletedTask);

            // Act
            await _learningMaterialService.DeleteLearningMaterialAsync(learningMaterialId, token);

            // Assert
            _learningMaterialClientMock.Verify(client => client.DeleteLearningMaterialAsync(learningMaterialId), Times.Once);
        }

        [Fact]
        public async Task DeleteLearningMaterialAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1L;
            var learningMaterialId = 1L;
            var existingLearningMaterial = new LearningMaterial { Id = learningMaterialId, UserId = 2L };

            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(userId);
            _learningMaterialClientMock.Setup(client => client.GetLearningMaterialByIdAsync(learningMaterialId)).ReturnsAsync(existingLearningMaterial);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _learningMaterialService.DeleteLearningMaterialAsync(learningMaterialId, token));
            Assert.Equal("You are not authorized to delete this learning material.", exception.Message);
        }

        [Fact]
        public async Task GetAllLearningMaterialsAsync_ShouldReturnAllLearningMaterials()
        {
            // Arrange
            var expectedLearningMaterials = new List<LearningMaterial>
            {
                new LearningMaterial { Headline = "Test Headline 1" },
                new LearningMaterial { Headline = "Test Headline 2" }
            };

            _learningMaterialClientMock.Setup(client => client.GetAllLearningMaterialsAsync()).ReturnsAsync(expectedLearningMaterials);

            // Act
            var result = await _learningMaterialService.GetAllLearningMaterialsAsync();

            // Assert
            Assert.Equal(expectedLearningMaterials.Count, result.Count);
            Assert.Equal(expectedLearningMaterials, result);
        }

        [Fact]
        public async Task GetLearningMaterialByIdAsync_ShouldReturnLearningMaterial_WhenLearningMaterialExists()
        {
            // Arrange
            var learningMaterialId = 1L;
            var expectedLearningMaterial = new LearningMaterial { Id = learningMaterialId, Headline = "Test Headline" };

            _learningMaterialClientMock.Setup(client => client.GetLearningMaterialByIdAsync(learningMaterialId)).ReturnsAsync(expectedLearningMaterial);

            // Act
            var result = await _learningMaterialService.GetLearningMaterialByIdAsync(learningMaterialId);

            // Assert
            Assert.Equal(expectedLearningMaterial, result);
        }

        [Fact]
        public async Task GetLearningMaterialByIdAsync_ShouldReturnNull_WhenLearningMaterialDoesNotExist()
        {
            // Arrange
            var learningMaterialId = 1L;

            _learningMaterialClientMock.Setup(client => client.GetLearningMaterialByIdAsync(learningMaterialId)).ReturnsAsync((LearningMaterial)null);

            // Act
            var result = await _learningMaterialService.GetLearningMaterialByIdAsync(learningMaterialId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateLearningMaterialAsync_ShouldReturnUpdatedLearningMaterial_WhenUpdateIsSuccessful()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1L;
            var learningMaterialId = 1L;
            var createLearningMaterialDto = new CreateLearningMaterial
            {
                Headline = "Updated Headline",
                Description = "Updated Description",
                Contents = new List<Content> { new TextContent { Body = "Updated Content" } }
            };
            var existingLearningMaterial = new LearningMaterial { Id = learningMaterialId, UserId = userId };
            var updatedLearningMaterial = new LearningMaterial { Id = learningMaterialId, Headline = createLearningMaterialDto.Headline };

            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(userId);
            _learningMaterialClientMock.Setup(client => client.GetLearningMaterialByIdAsync(learningMaterialId)).ReturnsAsync(existingLearningMaterial);
            _learningMaterialClientMock.Setup(client => client.UpdateLearningMaterialAsync(learningMaterialId, createLearningMaterialDto)).ReturnsAsync(updatedLearningMaterial);

            // Act
            var result = await _learningMaterialService.UpdateLearningMaterialAsync(learningMaterialId, createLearningMaterialDto, token);

            // Assert
            Assert.Equal(updatedLearningMaterial, result);
        }

        [Fact]
        public async Task UpdateLearningMaterialAsync_ShouldThrowArgumentException_WhenContentsAreEmpty()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1L;
            var learningMaterialId = 1L;
            var createLearningMaterialDto = new CreateLearningMaterial
            {
                Headline = "Updated Headline",
                Description = "Updated Description",
                Contents = new List<Content>()
            };
            var existingLearningMaterial = new LearningMaterial { Id = learningMaterialId, UserId = userId };

            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(userId);
            _learningMaterialClientMock.Setup(client => client.GetLearningMaterialByIdAsync(learningMaterialId)).ReturnsAsync(existingLearningMaterial);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _learningMaterialService.UpdateLearningMaterialAsync(learningMaterialId, createLearningMaterialDto, token));
            Assert.Equal("Learning material cannot be updated without at least one content.", exception.Message);
        }

        [Fact]
        public async Task UpdateLearningMaterialAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1L;
            var learningMaterialId = 1L;
            var createLearningMaterialDto = new CreateLearningMaterial
            {
                Headline = "Updated Headline",
                Description = "Updated Description",
                Contents = new List<Content> { new TextContent { Body = "Updated Content" } }
            };
            var existingLearningMaterial = new LearningMaterial { Id = learningMaterialId, UserId = 2L };

            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(userId);
            _learningMaterialClientMock.Setup(client => client.GetLearningMaterialByIdAsync(learningMaterialId)).ReturnsAsync(existingLearningMaterial);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _learningMaterialService.UpdateLearningMaterialAsync(learningMaterialId, createLearningMaterialDto, token));
            Assert.Equal("You are not authorized to update this learning material.", exception.Message);
        }
    }
}
