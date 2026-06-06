namespace ECommerce.Domain.Exceptions;

/// <summary>
/// Thrown when a user tries to access or modify another user's resource.
/// Mapped to HTTP 403 by the global exception handler.
/// </summary>
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to access this resource.")
        : base(message)
    {
    }
}
