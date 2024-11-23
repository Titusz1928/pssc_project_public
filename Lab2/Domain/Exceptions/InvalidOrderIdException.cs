namespace Lab2.Domain.Exceptions;

public class InvalidOrderIdException: Exception
{
    public InvalidOrderIdException() { }
    public InvalidOrderIdException(string? message) : base(message) { }
    public InvalidOrderIdException(string? message, Exception? innerException) : base(message, innerException) { }

}