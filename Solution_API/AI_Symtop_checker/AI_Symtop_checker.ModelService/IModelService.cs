// AI_Symtop_checker.ModelService/IModelService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_Symtop_checker.ModelService
{
    public interface IModelService
    {
        Task<ModelResponse> AnalyzeSymptomsAsync(ModelRequest request);
    }

    public class ModelRequest
    {
        public List<string> Symptoms { get; set; }
        public string PatientAge { get; set; }
        public string PatientGender { get; set; }
        public string AdditionalNotes { get; set; }
    }

    public class ModelResponse
    {
        public string Prediction { get; set; }
        public double Confidence { get; set; }
        public List<string> PossibleConditions { get; set; }
        public string UrgencyLevel { get; set; }
        public List<string> RecommendedActions { get; set; }
        public string AdditionalNotes { get; set; }
        public DateTime Timestamp { get; set; }
    }
}