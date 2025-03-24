using AI_Symtop_checker.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_Symtop_checker.Services.Interfaces
{
    public interface IAdminService
    {
        // Symptom management
        Task<List<Symptom>> GetAllSymptomsAsync();
        Task<Symptom> GetSymptomByIdAsync(Guid id);
        Task<Symptom> AddSymptomAsync(Symptom symptom);
        Task<bool> UpdateSymptomAsync(Symptom symptom);
        Task<bool> DeleteSymptomAsync(Guid id);

        // Prediction management
        Task<List<SymptomCheckPrediction>> GetPredictionsAsync(DateTime? startDate, DateTime? endDate, int page, int pageSize);
        Task<int> GetPredictionsCountAsync(DateTime? startDate, DateTime? endDate);
        Task<SymptomCheckPrediction> GetPredictionByIdAsync(Guid id);
        Task<bool> DeletePredictionAsync(Guid id);

        // Dashboard statistics
        Task<DashboardStats> GetDashboardStatsAsync();
    }
}