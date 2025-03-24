using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AI_Symtop_checker.Model
{
    public class SymptomCheckPrediction
    {
        [BsonId]
        public Guid Id { get; set; }

        public List<string> Symptoms { get; set; } = new List<string>();

        public string Prediction { get; set; } = string.Empty;

        public double Confidence { get; set; }

        public DateTime TimestampUtc { get; set; }

        public List<string> PossibleConditions { get; set; } = new List<string>();

        public string UrgencyLevel { get; set; } = string.Empty;

        public List<string> RecommendedActions { get; set; } = new List<string>();

        public string AdditionalNotes { get; set; } = string.Empty;
    }
}