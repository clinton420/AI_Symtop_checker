using AI_Symtop_checker.Model;

namespace AI_Symtop_checker.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateSymptom(Symptom symptom)
        {
            if (symptom == null)
                throw new ArgumentNullException(nameof(symptom));

            if (string.IsNullOrWhiteSpace(symptom.Name))
                throw new ArgumentException("Symptom name cannot be empty");

            if (symptom.Description?.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters");
        }

        public static void ValidateSymptomCheckRequest(SymptomCheckRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Symptoms == null || !request.Symptoms.Any())
                throw new ArgumentException("At least one symptom must be provided");

            if (request.Symptoms.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Symptom names cannot be empty");
        }
    }
}