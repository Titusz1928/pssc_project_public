namespace Lab2.Domain.Exceptions
{
    public class InvalidOrderStateException : Exception
    {
        public InvalidOrderStateException()
        {
        }

        public InvalidOrderStateException(string state) : base($"State {state} not implemented")
        {
        }

        public InvalidOrderStateException(string state, Exception innerException) : base($"State {state} not implemented", innerException)
        {
        }
    }
}