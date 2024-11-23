namespace Lab2.Domain.Exceptions;

public class InvalidTotalException:Exception
{
    public InvalidTotalException() { }
    public InvalidTotalException(string? message) : base(message) { }
    public InvalidTotalException(string? message, Exception? innerException) : base(message, innerException) { }

    
}