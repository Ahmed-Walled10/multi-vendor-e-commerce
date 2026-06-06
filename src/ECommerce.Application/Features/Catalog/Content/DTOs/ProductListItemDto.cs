namespace ECommerce.Application.Features.Catalog.Content.DTOs;

/// <summary>
/// Lightweight DTO used in paginated product lists (no nested attributes).
/// </summary>
public class ProductListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = default!;
    public decimal BasePrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int VariantCount { get; set; }
}
