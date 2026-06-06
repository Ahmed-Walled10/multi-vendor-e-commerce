namespace ECommerce.Domain.Exceptions;

/// <summary>
/// Base exception for domain-level errors.
/// </summary>
public abstract class AppException : Exception
{
    protected AppException(string message) : base(message) { }
    protected AppException(string message, Exception innerException) : base(message, innerException) { }
}
