using AI_Symtop_checker.Model;
using AI_Symtop_checker.Repositories.Interfaces;
using AI_Symtop_checker.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace AI_Symtop_checker.Services.Implementations
{
    public class SymptomCheckerService : ISymptomCheckerService
    {
        private readonly ISymptomRepository _symptomRepository;
        private readonly ILlamaIntegrationRepository _llamaRepository;
        private readonly IPredictionRepository _predictionRepository;
        private readonly ILogger<SymptomCheckerService> _logger;

        public SymptomCheckerService(
            ISymptomRepository symptomRepository,
            ILlamaIntegrationRepository llamaRepository,
            IPredictionRepository predictionRepository,
            ILogger<SymptomCheckerService> logger)
        {
            _symptomRepository = symptomRepository;
            _llamaRepository = llamaRepository;
            _predictionRepository = predictionRepository;
            _logger = logger;
        }

        public async Task<List<Symptom>> GetAllSymptomsAsync()
        {
            try
            {
                return await _symptomRepository.GetAllSymptomsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all symptoms");
                throw;
            }
        }

        public async Task<Symptom> GetSymptomByIdAsync(Guid id)
        {
            try
            {
                var symptom = await _symptomRepository.GetSymptomByIdAsync(id);
                if (symptom == null)
                {
                    throw new KeyNotFoundException($"Symptom with ID {id} not found");
                }
                return symptom;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving symptom with ID {SymptomId}", id);
                throw;
            }
        }

        public async Task<List<Symptom>> GetSymptomsByNameAsync(List<string> symptomNames)
        {
            try
            {
                return await _symptomRepository.GetSymptomsByNameAsync(symptomNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving symptoms by names");
                throw;
            }
        }

        public async Task<Symptom> AddSymptomAsync(Symptom symptom)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symptom.Name))
                {
                    throw new ArgumentException("Symptom name cannot be empty");
                }

                return await _symptomRepository.AddSymptomAsync(symptom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new symptom");
                throw;
            }
        }

        public async Task<bool> UpdateSymptomAsync(Symptom symptom)
        {
            try
            {
                if (symptom.Id == Guid.Empty)
                {
                    throw new ArgumentException("Symptom ID cannot be empty");
                }

                return await _symptomRepository.UpdateSymptomAsync(symptom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating symptom with ID {SymptomId}", symptom.Id);
                throw;
            }
        }

        public async Task<SymptomCheckPrediction> CheckSymptomsAsync(List<string> symptoms)
        {
            if (symptoms == null || !symptoms.Any())
            {
                throw new ArgumentException("At least one symptom must be provided");
            }

            var request = new SymptomCheckRequest
            {
                Symptoms = symptoms
            };

            return await GetSymptomPredictionAsync(request);
        }

        public async Task<bool> DeleteSymptomAsync(Guid id)
        {
            try
            {
                return await _symptomRepository.DeleteSymptomAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting symptom with ID {SymptomId}", id);
                throw;
            }
        }

        public async Task<SymptomCheckPrediction> GetSymptomPredictionAsync(SymptomCheckRequest request)
        {
            try
            {
                if (request.Symptoms == null || !request.Symptoms.Any())
                {
                    throw new ArgumentException("At least one symptom must be provided");
                }

                // Get prediction from Llama model
                _logger.LogInformation("Getting prediction for symptoms: {Symptoms}", string.Join(", ", request.Symptoms));
                var prediction = await _llamaRepository.GetPredictionAsync(request);

                // Save the prediction to database for future reference and dashboard stats
                try
                {
                    if (_predictionRepository != null)
                    {
                        await _predictionRepository.AddPredictionAsync(prediction);
                        _logger.LogInformation("Prediction saved successfully with ID: {PredictionId}", prediction.Id);
                    }
                }
                catch (Exception ex)
                {
                    // Just log the error but don't fail the request if saving fails
                    _logger.LogWarning(ex, "Failed to save prediction to database, but prediction was generated successfully");
                }

                return prediction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prediction for symptoms: {Symptoms}",
                    request.Symptoms != null ? string.Join(", ", request.Symptoms) : "null");
                throw;
            }
        }
    }
}