using AI_Symtop_checker.Model;
using AI_Symtop_checker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SymptomCheckerController : ControllerBase
{
    private readonly ISymptomCheckerService _service;
    private readonly ILogger<SymptomCheckerController> _logger;

    public SymptomCheckerController(ISymptomCheckerService service, ILogger<SymptomCheckerController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("check")]
    public async Task<ActionResult<SymptomCheckPrediction>> CheckSymptomsAsync([FromBody] SymptomCheckRequest request)
    {
        try
        {
            var prediction = await _service.CheckSymptomsAsync(request.Symptoms);
            return Ok(prediction);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid symptom check request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Symptom check processing error");
            return StatusCode(500, "Internal server error");
        }
    }
}