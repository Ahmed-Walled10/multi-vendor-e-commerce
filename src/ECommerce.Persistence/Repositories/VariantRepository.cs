using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.ResourceParameters;
using ECommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class VariantRepository : IVariantRepository
{
    private readonly ApplicationDbContext _context;

    public VariantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductVariant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<ProductVariant?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariants
            .Include(v => v.VariantAttributeValues)
                .ThenInclude(vav => vav.AttributeValue)
                    .ThenInclude(av => av.Attribute)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<ProductVariant> Items, int TotalCount)> GetPagedAsync(
        Guid productId,
        VariantResourceParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProductVariants
            .Include(v => v.VariantAttributeValues)
                .ThenInclude(vav => vav.AttributeValue)
                    .ThenInclude(av => av.Attribute)
            .Where(v => v.ProductId == productId);

        if (parameters.IsActive.HasValue)
            query = query.Where(v => v.IsActive == parameters.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(parameters.Sku))
            query = query.Where(v => v.SKU.ToLower().Contains(parameters.Sku.ToLower()));

        query = query.OrderByDescending(v => v.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> SkuExistsAsync(string sku, Guid? excludeVariantId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ProductVariants.Where(v => v.SKU == sku);

        if (excludeVariantId.HasValue)
            query = query.Where(v => v.Id != excludeVariantId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(ProductVariant variant, CancellationToken cancellationToken = default)
    {
        await _context.ProductVariants.AddAsync(variant, cancellationToken);
    }

    public void Update(ProductVariant variant)
    {
        _context.ProductVariants.Update(variant);
    }

    public void Delete(ProductVariant variant)
    {
        _context.ProductVariants.Remove(variant);
    }
}
