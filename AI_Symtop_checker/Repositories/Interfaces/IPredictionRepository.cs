using AI_Symtop_checker.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_Symtop_checker.Repositories.Interfaces
{
    public interface IPredictionRepository
    {
        Task<List<SymptomCheckPrediction>> GetPredictionsAsync(DateTime? startDate, DateTime? endDate, int page, int pageSize);
        Task<int> GetPredictionsCountAsync(DateTime? startDate, DateTime? endDate);
        Task<int> GetPredictionsCountByUrgencyLevelAsync(string urgencyLevel);
        Task<SymptomCheckPrediction> GetPredictionByIdAsync(Guid id);
        Task<SymptomCheckPrediction> AddPredictionAsync(SymptomCheckPrediction prediction);
        Task<bool> DeletePredictionAsync(Guid id);
    }
}