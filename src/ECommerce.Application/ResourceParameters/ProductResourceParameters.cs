using ECommerce.Domain.Common.Enums;

namespace ECommerce.Application.ResourceParameters;

/// <summary>
/// Query parameters for the product catalogue list endpoint.
/// </summary>
public class ProductResourceParameters : BaseResourceParameters
{

    public ProductStatus? Status { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; } = true;
}
