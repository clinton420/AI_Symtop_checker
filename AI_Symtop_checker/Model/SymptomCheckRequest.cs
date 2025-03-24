namespace AI_Symtop_checker.Model
{
    public class SymptomCheckRequest
    {
        public List<string> Symptoms { get; set; } = new List<string>();
        public string PatientAge { get; set; } = string.Empty;
        public string PatientGender { get; set; } = string.Empty;
        public string AdditionalNotes { get; set; } = string.Empty;
    }
}