using System;

namespace AI_Symtop_checker.Model
{
    public class DashboardStats
    {
        public int TotalPredictions { get; set; }
        public int TodayPredictions { get; set; }
        public int HighUrgencyCount { get; set; }
        public int UniqueSymptomsCount { get; set; }
        public double AverageConfidence { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}