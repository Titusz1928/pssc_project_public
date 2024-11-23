using System;

namespace Lab2.Domain.Exceptions
{
    internal class InvalidProductCodeException : Exception
    {
        public InvalidProductCodeException() { }
        public InvalidProductCodeException(string? message) : base(message) { }
        public InvalidProductCodeException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}