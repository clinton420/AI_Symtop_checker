using Microsoft.AspNetCore.Mvc;
using AI_Symtop_checker.Services.Interfaces;
using AI_Symtop_checker.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_Symtop_checker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet("symptoms")]
        public async Task<ActionResult<List<Symptom>>> GetAllSymptomsAsync()
        {
            try
            {
                var symptoms = await _adminService.GetAllSymptomsAsync();
                return Ok(symptoms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all symptoms");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("symptoms/{id}")]
        public async Task<ActionResult<Symptom>> GetSymptomByIdAsync(Guid id)
        {
            try
            {
                var symptom = await _adminService.GetSymptomByIdAsync(id);
                if (symptom == null)
                {
                    return NotFound($"Symptom with ID {id} not found");
                }
                return Ok(symptom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving symptom with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("symptoms")]
        public async Task<ActionResult<Symptom>> AddSymptomAsync(Symptom symptom)
        {
            try
            {
                if (symptom.Id == Guid.Empty)
                {
                    symptom.Id = Guid.NewGuid();
                }

                var addedSymptom = await _adminService.AddSymptomAsync(symptom);
                return CreatedAtAction(nameof(GetSymptomByIdAsync), new { id = addedSymptom.Id }, addedSymptom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new symptom");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("symptoms/{id}")]
        public async Task<ActionResult> UpdateSymptomAsync(Guid id, Symptom symptom)
        {
            try
            {
                if (id != symptom.Id)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var exists = await _adminService.GetSymptomByIdAsync(id);
                if (exists == null)
                {
                    return NotFound($"Symptom with ID {id} not found");
                }

                var success = await _adminService.UpdateSymptomAsync(symptom);
                if (!success)
                {
                    return StatusCode(500, "Failed to update symptom");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating symptom with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("symptoms/{id}")]
        public async Task<ActionResult> DeleteSymptomAsync(Guid id)
        {
            try
            {
                var exists = await _adminService.GetSymptomByIdAsync(id);
                if (exists == null)
                {
                    return NotFound($"Symptom with ID {id} not found");
                }

                var success = await _adminService.DeleteSymptomAsync(id);
                if (!success)
                {
                    return StatusCode(500, "Failed to delete symptom");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting symptom with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("predictions")]
        public async Task<ActionResult<List<SymptomCheckPrediction>>> GetPredictionsAsync(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var predictions = await _adminService.GetPredictionsAsync(startDate, endDate, page, pageSize);

                // Get total count for pagination
                var totalCount = await _adminService.GetPredictionsCountAsync(startDate, endDate);

                Response.Headers.Add("X-Total-Count", totalCount.ToString());

                return Ok(predictions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving predictions");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("predictions/{id}")]
        public async Task<ActionResult<SymptomCheckPrediction>> GetPredictionByIdAsync(Guid id)
        {
            try
            {
                var prediction = await _adminService.GetPredictionByIdAsync(id);
                if (prediction == null)
                {
                    return NotFound($"Prediction with ID {id} not found");
                }
                return Ok(prediction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving prediction with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("predictions/{id}")]
        public async Task<ActionResult> DeletePredictionAsync(Guid id)
        {
            try
            {
                var exists = await _adminService.GetPredictionByIdAsync(id);
                if (exists == null)
                {
                    return NotFound($"Prediction with ID {id} not found");
                }

                var success = await _adminService.DeletePredictionAsync(id);
                if (!success)
                {
                    return StatusCode(500, "Failed to delete prediction");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting prediction with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("dashboard/stats")]
        public async Task<ActionResult<DashboardStats>> GetDashboardStatsAsync()
        {
            try
            {
                var stats = await _adminService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard statistics");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}