using System.Text.Json.Serialization;

namespace AI_Symtop_checker.Model
{
    public class LlamaApiResponse
    {
        [JsonPropertyName("prediction")]
        public string Prediction { get; set; } = string.Empty;

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("possible_conditions")]
        public List<string> PossibleConditions { get; set; } = new List<string>();

        [JsonPropertyName("urgency_level")]
        public string UrgencyLevel { get; set; } = string.Empty;

        [JsonPropertyName("recommended_actions")]
        public List<string> RecommendedActions { get; set; } = new List<string>();

        [JsonPropertyName("additional_notes")]
        public string AdditionalNotes { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}