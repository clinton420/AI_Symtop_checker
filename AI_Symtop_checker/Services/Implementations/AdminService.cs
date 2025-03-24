using AI_Symtop_checker.Model;
using AI_Symtop_checker.Repositories.Interfaces;
using AI_Symtop_checker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace AI_Symtop_checker.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly ISymptomRepository _symptomRepository;
        private readonly IPredictionRepository _predictionRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            ISymptomRepository symptomRepository,
            IPredictionRepository predictionRepository,
            ILogger<AdminService> logger)
        {
            _symptomRepository = symptomRepository;
            _predictionRepository = predictionRepository;
            _logger = logger;
        }

        // Symptom management
        public async Task<List<Symptom>> GetAllSymptomsAsync()
        {
            try
            {
                return await _symptomRepository.GetAllSymptomsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllSymptomsAsync");
                throw;
            }
        }

        public async Task<Symptom> GetSymptomByIdAsync(Guid id)
        {
            try
            {
                return await _symptomRepository.GetSymptomByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetSymptomByIdAsync for id: {id}");
                throw;
            }
        }

        public async Task<Symptom> AddSymptomAsync(Symptom symptom)
        {
            try
            {
                return await _symptomRepository.AddSymptomAsync(symptom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddSymptomAsync");
                throw;
            }
        }

        public async Task<bool> UpdateSymptomAsync(Symptom symptom)
        {
            try
            {
                return await _symptomRepository.UpdateSymptomAsync(symptom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateSymptomAsync for id: {symptom.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteSymptomAsync(Guid id)
        {
            try
            {
                return await _symptomRepository.DeleteSymptomAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in DeleteSymptomAsync for id: {id}");
                throw;
            }
        }

        // Prediction management
        public async Task<List<SymptomCheckPrediction>> GetPredictionsAsync(DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            try
            {
                return await _predictionRepository.GetPredictionsAsync(startDate, endDate, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPredictionsAsync");
                throw;
            }
        }

        public async Task<int> GetPredictionsCountAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                return await _predictionRepository.GetPredictionsCountAsync(startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPredictionsCountAsync");
                throw;
            }
        }

        public async Task<SymptomCheckPrediction> GetPredictionByIdAsync(Guid id)
        {
            try
            {
                return await _predictionRepository.GetPredictionByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetPredictionByIdAsync for id: {id}");
                throw;
            }
        }

        public async Task<bool> DeletePredictionAsync(Guid id)
        {
            try
            {
                return await _predictionRepository.DeletePredictionAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in DeletePredictionAsync for id: {id}");
                throw;
            }
        }

        // Dashboard statistics
        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            try
            {
                // Get total predictions count
                var totalPredictions = await _predictionRepository.GetPredictionsCountAsync(null, null);

                // Get predictions count for today
                var today = DateTime.UtcNow.Date;
                var tomorrow = today.AddDays(1);
                var todayPredictions = await _predictionRepository.GetPredictionsCountAsync(today, tomorrow);

                // Get predictions with high urgency
                var highUrgencyCount = await _predictionRepository.GetPredictionsCountByUrgencyLevelAsync("High");

                // Get total unique symptoms count
                var symptoms = await _symptomRepository.GetAllSymptomsAsync();
                var uniqueSymptomsCount = symptoms.Count;

                // Get average confidence
                var recentPredictions = await _predictionRepository.GetPredictionsAsync(null, null, 1, 100);
                var avgConfidence = recentPredictions.Any() ? recentPredictions.Average(p => p.Confidence) : 0;

                // Create dashboard stats
                var stats = new DashboardStats
                {
                    TotalPredictions = totalPredictions,
                    TodayPredictions = todayPredictions,
                    HighUrgencyCount = highUrgencyCount,
                    UniqueSymptomsCount = uniqueSymptomsCount,
                    AverageConfidence = avgConfidence
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDashboardStatsAsync");
                throw;
            }
        }
    }
}