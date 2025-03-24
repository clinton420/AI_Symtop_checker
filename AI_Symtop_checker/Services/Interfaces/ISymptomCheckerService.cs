using AI_Symtop_checker.Model;

namespace AI_Symtop_checker.Services.Interfaces
{
    public interface ISymptomCheckerService
    {
        Task<List<Symptom>> GetAllSymptomsAsync();
        Task<Symptom> GetSymptomByIdAsync(Guid id);
        Task<List<Symptom>> GetSymptomsByNameAsync(List<string> symptomNames);
        Task<Symptom> AddSymptomAsync(Symptom symptom);
        Task<bool> UpdateSymptomAsync(Symptom symptom);
        Task<bool> DeleteSymptomAsync(Guid id);
        Task<SymptomCheckPrediction> GetSymptomPredictionAsync(SymptomCheckRequest request);

        Task<SymptomCheckPrediction> CheckSymptomsAsync(List<string> symptoms);
    }
}