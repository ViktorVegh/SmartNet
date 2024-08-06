using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Models.DTOs;

namespace IClients
{
    public interface ILearningMaterialClient
    {
        Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterial);
        Task<LearningMaterial> GetLearningMaterialByIdAsync(long id);
        Task<List<LearningMaterial>> GetAllLearningMaterialsAsync();
        Task<LearningMaterial> UpdateLearningMaterialAsync(long id, CreateLearningMaterial learningMaterial);
        Task DeleteLearningMaterialAsync(long id);
    }
}