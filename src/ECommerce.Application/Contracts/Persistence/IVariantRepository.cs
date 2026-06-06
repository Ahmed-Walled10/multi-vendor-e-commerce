using ECommerce.Domain.Entities.Catalog;
using ECommerce.Application.ResourceParameters;

namespace ECommerce.Application.Contracts.Persistence;

public interface IVariantRepository
{
    Task<ProductVariant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProductVariant?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ProductVariant> Items, int TotalCount)> GetPagedAsync(
        Guid productId,
        VariantResourceParameters parameters,
        CancellationToken cancellationToken = default);

    Task<bool> SkuExistsAsync(string sku, Guid? excludeVariantId = null, CancellationToken cancellationToken = default);
    Task AddAsync(ProductVariant variant, CancellationToken cancellationToken = default);
    void Update(ProductVariant variant);
    void Delete(ProductVariant variant);
}
