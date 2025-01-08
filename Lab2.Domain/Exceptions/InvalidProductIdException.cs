namespace Lab2.Domain.Exceptions;

public class InvalidProductIdException:Exception
{
    public InvalidProductIdException() { }
    public InvalidProductIdException(string? message) : base(message) { }
    public InvalidProductIdException(string? message, Exception? innerException) : base(message, innerException) { }
}