namespace AI_Symtop_checker.Exceptions
{
    public class SymptomNotFoundException : Exception
    {
        public Guid SymptomId { get; }

        public SymptomNotFoundException(Guid id)
            : base($"Symptom with ID {id} was not found.")
        {
            SymptomId = id;
        }

        public SymptomNotFoundException(string message)
            : base(message)
        {
        }
    }
}