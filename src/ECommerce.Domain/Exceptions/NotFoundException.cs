namespace ECommerce.Domain.Exceptions;

/// <summary>
/// Thrown when a requested entity is not found.
/// Mapped to HTTP 404 by the global exception handler.
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
