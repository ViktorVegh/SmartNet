using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Models.DTOs;

namespace IWebSocketService
{
    public interface ILearningMaterialWebSocketService
    {
        Task ConnectAsync();
        Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterial, string token);
        Task<LearningMaterial> GetLearningMaterialByIdAsync(long id);
        Task<List<LearningMaterial>> GetAllLearningMaterialsAsync();
        Task<LearningMaterial> UpdateLearningMaterialAsync(long id, LearningMaterial learningMaterial, string token); // Updated this line
        Task<string> DeleteLearningMaterialAsync(long id, string token);
        Task<List<LearningMaterial>> GetLearningMaterialsByHeadlineAsync(string headlineSubstring);
        Task CloseAsync();
    }
}