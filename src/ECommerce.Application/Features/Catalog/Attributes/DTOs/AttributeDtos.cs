namespace ECommerce.Application.Features.Catalog.Attributes.DTOs;

public class AttributeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public List<AttributeValueDto> Values { get; set; } = new();
}

public class AttributeValueDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = default!;
}
