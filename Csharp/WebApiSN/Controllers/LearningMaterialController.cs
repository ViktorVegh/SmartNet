using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IServices;
using Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.DTOs;

namespace WebApiSN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningMaterialsController : ControllerBase
    {
        private readonly ILearningMaterialService _learningMaterialService;
        private readonly ILogger<LearningMaterialsController> _logger;

        public LearningMaterialsController(ILearningMaterialService learningMaterialService, ILogger<LearningMaterialsController> logger)
        {
            _learningMaterialService = learningMaterialService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddLearningMaterial([FromBody] CreateLearningMaterial learningMaterial)
        {
            _logger.LogInformation("Received DTO: {@LearningMaterialDto}", learningMaterial);

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            _logger.LogInformation("Controller - Token received: {Token}", token);

            try
            {
                var createdLearningMaterial = await _learningMaterialService.AddLearningMaterialAsync(learningMaterial, token);
                _logger.LogInformation("Controller - Learning Material Created: {@LearningMaterial}", createdLearningMaterial);
                return Ok(createdLearningMaterial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller - Error Creating Learning Material");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetLearningMaterialById(long id)
        {
            try
            {
                var learningMaterial = await _learningMaterialService.GetLearningMaterialByIdAsync(id);
                if (learningMaterial == null)
                {
                    return NotFound();
                }
                return Ok(learningMaterial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller - Error Getting Learning Material by ID");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLearningMaterials()
        {
            try
            {
                var materials = await _learningMaterialService.GetAllLearningMaterialsAsync();
                return Ok(materials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller - Error Getting All Learning Materials");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateLearningMaterial(long id, [FromBody] CreateLearningMaterial learningMaterial)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {
                var updatedLearningMaterial = await _learningMaterialService.UpdateLearningMaterialAsync(id, learningMaterial, token);
                return Ok(updatedLearningMaterial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller - Error Updating Learning Material");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteLearningMaterial(long id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {
                await _learningMaterialService.DeleteLearningMaterialAsync(id, token);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller - Error Deleting Learning Material");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
