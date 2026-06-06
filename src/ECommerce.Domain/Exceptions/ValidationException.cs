using FluentValidation.Results;

namespace ECommerce.Domain.Exceptions;

/// <summary>
/// Thrown by the ValidationBehavior when FluentValidation finds errors.
/// Mapped to HTTP 400 with per-field error details by the global exception handler.
/// </summary>
public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
