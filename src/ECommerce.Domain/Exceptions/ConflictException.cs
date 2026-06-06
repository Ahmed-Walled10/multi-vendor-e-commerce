namespace ECommerce.Domain.Exceptions;

/// <summary>
/// Thrown when an operation conflicts with existing state (e.g., duplicate SKU).
/// Mapped to HTTP 409 by the global exception handler.
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
