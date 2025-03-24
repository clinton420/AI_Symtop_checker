using System.Collections.Generic;
using System.Threading.Tasks;
using AI_Symtop_checker.Model;

namespace AI_Symtop_checker.Repositories.Interfaces
{
    public interface ILlamaIntegrationRepository
    {
        
        Task<SymptomCheckPrediction> GetPredictionAsync(SymptomCheckRequest request);
           
    }
}

