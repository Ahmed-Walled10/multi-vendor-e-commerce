using ECommerce.Application.ResourceParameters;
using ECommerce.Domain.Entities.Catalog;

namespace ECommerce.Application.Contracts.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        string userId,
        ProductResourceParameters parameters,
        CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    void Delete(Product product);

    Task AddAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
    Task AddAttributeValueAsync(ProductAttributeValue value, CancellationToken cancellationToken = default);
}
