using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.ResourceParameters;
using ECommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Attributes)
                .ThenInclude(a => a.Values)
            .Include(p => p.Variants)
                .ThenInclude(v => v.VariantAttributeValues)
                    .ThenInclude(vav => vav.AttributeValue)
                        .ThenInclude(av => av.Attribute)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        string userId,
        ProductResourceParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .Include(p => p.Variants)
            .Where(p => p.UserId == userId);

        // ── Search ───────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
        {
            var term = parameters.SearchQuery.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                (p.Description != null && p.Description.ToLower().Contains(term)));
        }

        // ── Filters ──────────────────────────────────────────────────
        if (parameters.Status.HasValue)
            query = query.Where(p => p.Status == parameters.Status.Value);

        if (parameters.MinPrice.HasValue)
            query = query.Where(p => p.BasePrice >= parameters.MinPrice.Value);

        if (parameters.MaxPrice.HasValue)
            query = query.Where(p => p.BasePrice <= parameters.MaxPrice.Value);

        if (parameters.FromDate.HasValue)
            query = query.Where(p => p.CreatedAt >= parameters.FromDate.Value);

        if (parameters.ToDate.HasValue)
            query = query.Where(p => p.CreatedAt <= parameters.ToDate.Value);

        // ── Sorting ──────────────────────────────────────────────────
        query = (parameters.SortBy?.ToLowerInvariant(), parameters.SortDescending) switch
        {
            ("name",      true)  => query.OrderByDescending(p => p.Name),
            ("name",      false) => query.OrderBy(p => p.Name),
            ("price",     true)  => query.OrderByDescending(p => p.BasePrice),
            ("price",     false) => query.OrderBy(p => p.BasePrice),
            ("updatedat", true)  => query.OrderByDescending(p => p.UpdatedAt),
            ("updatedat", false) => query.OrderBy(p => p.UpdatedAt),
            ("status",    true)  => query.OrderByDescending(p => p.Status),
            ("status",    false) => query.OrderBy(p => p.Status),
            // default: createdat desc
            (_,           false) => query.OrderBy(p => p.CreatedAt),
            _                    => query.OrderByDescending(p => p.CreatedAt)
        };

        // ── Pagination ───────────────────────────────────────────────
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task AddAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
    {
        await _context.ProductAttributes.AddAsync(attribute, cancellationToken);
    }

    public async Task AddAttributeValueAsync(ProductAttributeValue value, CancellationToken cancellationToken = default)
    {
        await _context.ProductAttributeValues.AddAsync(value, cancellationToken);
    }
}
