namespace ECommerce.Application.Common.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = default!;
    public IDictionary<string, string[]>? Errors { get; set; }
}
