namespace AI_Symtop_checker.Configurations
{
    public class MongoDbConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = "SymptomCheckerDb";

        // Collection names for all models
        public string SymptomsCollectionName { get; set; } = "Symptoms";
        public string SymptomCheckPredictionsCollectionName { get; set; } = "SymptomCheckPredictions";
        public string SymptomCheckRequestsCollectionName { get; set; } = "SymptomCheckRequests";
        public string LlamaApiResponsesCollectionName { get; set; } = "LlamaApiResponses";

        // Validate the configuration
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ConnectionString) &&
                   !string.IsNullOrEmpty(DatabaseName) &&
                   !string.IsNullOrEmpty(SymptomsCollectionName) &&
                   !string.IsNullOrEmpty(SymptomCheckPredictionsCollectionName) &&
                   !string.IsNullOrEmpty(SymptomCheckRequestsCollectionName) &&
                   !string.IsNullOrEmpty(LlamaApiResponsesCollectionName);
        }
    }
}