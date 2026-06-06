namespace ECommerce.Application.ResourceParameters;

/// <summary>
/// Query parameters for listing product variants.
/// </summary>
public class VariantResourceParameters : BaseResourceParameters
{
    public bool? IsActive { get; set; }
    public string? Sku { get; set; }
}
