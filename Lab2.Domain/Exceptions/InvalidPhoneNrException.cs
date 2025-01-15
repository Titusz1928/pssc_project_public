namespace Lab2.Domain.Exceptions;

public class InvalidPhoneNrException: Exception
{
    public InvalidPhoneNrException() { }
    public InvalidPhoneNrException(string? message) : base(message) { }
    public InvalidPhoneNrException(string? message, Exception? innerException) : base(message, innerException) { }
}