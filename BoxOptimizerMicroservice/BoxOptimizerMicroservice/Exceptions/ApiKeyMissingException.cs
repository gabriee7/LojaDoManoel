namespace BoxOptimizerMicroservice.Exceptions
{
    public class ApiKeyMissingException : Exception
    {
        public ApiKeyMissingException(string message) : base(message) { }
    }
}
