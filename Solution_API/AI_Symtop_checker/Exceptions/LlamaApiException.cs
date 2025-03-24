namespace AI_Symtop_checker.Exceptions
{
    public class LlamaApiException : Exception
    {
        public LlamaApiException(string message) : base(message)
        {
        }

        public LlamaApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}