using System;

namespace Lab2.Domain.Exceptions
{
    internal class InvalidQuantityException : Exception
    {
        public InvalidQuantityException() { }
        public InvalidQuantityException(string? message) : base(message) { }
        public InvalidQuantityException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}