using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Models.DTOs;

namespace IHttpClientsSN
{
    public interface ILearningMaterialHttpClient
    {
        Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterial, string token);
        Task<LearningMaterial> GetLearningMaterialByIdAsync(long id);
        Task<List<LearningMaterial>> GetAllLearningMaterialsAsync();
        Task<LearningMaterial> UpdateLearningMaterialAsync(long id, LearningMaterial learningMaterial, string token);
        Task DeleteLearningMaterialAsync(long id, string token);
    }
}