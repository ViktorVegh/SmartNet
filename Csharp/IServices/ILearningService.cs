using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Models.DTOs;

namespace IServices
{
    public interface ILearningMaterialService
    {
        Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterial, string token);
        Task<LearningMaterial> GetLearningMaterialByIdAsync(long id);
        Task<List<LearningMaterial>> GetAllLearningMaterialsAsync();
        Task<LearningMaterial> UpdateLearningMaterialAsync(long id, CreateLearningMaterial learningMaterial, string token);
        Task DeleteLearningMaterialAsync(long id, string token);

        Task<List<LearningMaterial>> GetLearningMaterialsByHeadlineAsync(string headlineSubstring);
    }
}