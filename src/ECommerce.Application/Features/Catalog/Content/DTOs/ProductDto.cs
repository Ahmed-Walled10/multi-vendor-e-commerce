namespace ECommerce.Application.Features.Catalog.Content.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Status { get; set; } = default!;
    public decimal BasePrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ProductAttributeDto> Attributes { get; set; } = new();
    public int VariantCount { get; set; }
}

public class ProductAttributeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public List<ProductAttributeValueDto> Values { get; set; } = new();
}

public class ProductAttributeValueDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = default!;
}
