using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AI_Symtop_checker.Model;

namespace AI_Symtop_checker.Repositories.Interfaces
{
    public interface ISymptomRepository
    {
        Task<List<Symptom>> GetAllSymptomsAsync();
        Task<Symptom> GetSymptomByIdAsync(Guid id);
        Task<List<Symptom>> GetSymptomsByNameAsync(List<string> symptomNames);
        Task<Symptom> AddSymptomAsync(Symptom symptom);
        Task<bool> UpdateSymptomAsync(Symptom symptom);
        Task<bool> DeleteSymptomAsync(Guid id);

        Task<SymptomCheckPrediction> CheckSymptomsAsync(List<string> symptoms);
    }
}