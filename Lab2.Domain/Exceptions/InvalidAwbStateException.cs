namespace Lab2.Domain.Exceptions;

public class InvalidAwbStateException : Exception
{
    public InvalidAwbStateException()
    {
    }

    public InvalidAwbStateException(string state) : base($"State {state} not implemented")
    {
    }

    public InvalidAwbStateException(string state, Exception innerException) : base($"State {state} not implemented",
        innerException)
    {
    }
}