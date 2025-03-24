using System;
using System.Collections.Generic;
using System.Text.Json;
using AI_Symtop_checker.Exceptions;
using AI_Symtop_checker.Model;
using AI_Symtop_checker.Repositories.Interfaces;
using AI_Symtop_checker.ModelService;
using Microsoft.Extensions.Logging;

namespace AI_Symtop_checker.Repositories.Implementations
{
    public class LlamaIntegrationRepository : ILlamaIntegrationRepository
    {
        private readonly IModelService _modelService;
        private readonly ILogger<LlamaIntegrationRepository> _logger;

        public LlamaIntegrationRepository(
            IModelService modelService,
            ILogger<LlamaIntegrationRepository> logger)
        {
            _modelService = modelService;
            _logger = logger;
        }

        public async Task<SymptomCheckPrediction> GetPredictionAsync(SymptomCheckRequest request)
        {
            try
            {
                _logger.LogInformation("Getting prediction for symptoms: {Symptoms}",
                    string.Join(", ", request.Symptoms));

                // Convert to ModelRequest
                var modelRequest = new ModelRequest
                {
                    Symptoms = request.Symptoms,
                    PatientAge = request.PatientAge,
                    PatientGender = request.PatientGender,
                    AdditionalNotes = request.AdditionalNotes
                };

                // Call model service directly
                var modelResponse = await _modelService.AnalyzeSymptomsAsync(modelRequest);

                // Create prediction from response
                return new SymptomCheckPrediction
                {
                    Id = Guid.NewGuid(),
                    Symptoms = request.Symptoms,
                    Prediction = modelResponse.Prediction,
                    Confidence = modelResponse.Confidence,
                    PossibleConditions = modelResponse.PossibleConditions ?? new List<string>(),
                    UrgencyLevel = modelResponse.UrgencyLevel,
                    RecommendedActions = modelResponse.RecommendedActions ?? new List<string>(),
                    AdditionalNotes = modelResponse.AdditionalNotes,
                    TimestampUtc = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in model service");
                throw new LlamaApiException("Failed to get model prediction", ex);
            }
        }
    }
}