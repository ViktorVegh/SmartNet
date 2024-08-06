using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IClients;
using IServices;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;

namespace Services
{
    public class LearningMaterialService : ILearningMaterialService
    {
        private readonly ILearningMaterialClient _learningMaterialClient;
        private readonly ITokenHelper _tokenHelper;
        private readonly ILogger<LearningMaterialService> _logger;

        public LearningMaterialService(
            ILearningMaterialClient learningMaterialClient,
            ITokenHelper tokenHelper,
            ILogger<LearningMaterialService> logger)
        {
            _learningMaterialClient = learningMaterialClient;
            _tokenHelper = tokenHelper;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterialDto, string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);
            _logger.LogInformation("User ID Retrieved: {UserId}", userId);

            if (userId == 0)
            {
                throw new UnauthorizedAccessException("Only registered users can create content.");
            }

            if (learningMaterialDto.Contents == null || !learningMaterialDto.Contents.Any())
            {
                throw new ArgumentException("Learning material cannot be created without at least one content.");
            }

            learningMaterialDto.UserId = userId;
            learningMaterialDto.CreatedAt = DateTime.UtcNow;
            learningMaterialDto.UpdatedAt = DateTime.UtcNow;

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true,
                Converters = { new ContentJsonConverter() }
            };

            var json = JsonSerializer.Serialize(learningMaterialDto, options);
            _logger.LogInformation("Serialized JSON: {Json}", json);

            var savedMaterial = await _learningMaterialClient.AddLearningMaterialAsync(learningMaterialDto);
            return savedMaterial;
        }

        public async Task<LearningMaterial> GetLearningMaterialByIdAsync(long id)
        {
            var learningMaterial = await _learningMaterialClient.GetLearningMaterialByIdAsync(id);
            return learningMaterial;
        }

        public async Task<LearningMaterial> UpdateLearningMaterialAsync(long id, CreateLearningMaterial learningMaterialDto, string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);

            var existingLearningMaterial = await _learningMaterialClient.GetLearningMaterialByIdAsync(id);
            if (existingLearningMaterial == null || existingLearningMaterial.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this learning material.");
            }

            if (learningMaterialDto.Contents == null || !learningMaterialDto.Contents.Any())
            {
                throw new ArgumentException("Learning material cannot be updated without at least one content.");
            }

            existingLearningMaterial.Headline = learningMaterialDto.Headline;
            existingLearningMaterial.Description = learningMaterialDto.Description;
            existingLearningMaterial.MembersOnly = learningMaterialDto.MembersOnly;
            existingLearningMaterial.UpdatedAt = DateTime.UtcNow;

            
            foreach (var content in learningMaterialDto.Contents)
            {
                content.LearningMaterial = existingLearningMaterial;
            }

            existingLearningMaterial.Contents = learningMaterialDto.Contents;

            _logger.LogInformation("Updated Learning Material: {@LearningMaterial}", existingLearningMaterial);

            var updatedMaterial = await _learningMaterialClient.UpdateLearningMaterialAsync(id, learningMaterialDto);
            return updatedMaterial;
        }

        public async Task DeleteLearningMaterialAsync(long id, string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);

            var existingLearningMaterial = await _learningMaterialClient.GetLearningMaterialByIdAsync(id);
            if (existingLearningMaterial == null || existingLearningMaterial.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this learning material.");
            }

            await _learningMaterialClient.DeleteLearningMaterialAsync(id);
        }

        public async Task<List<LearningMaterial>> GetAllLearningMaterialsAsync()
        {
            return await _learningMaterialClient.GetAllLearningMaterialsAsync();
        }
    }
}
