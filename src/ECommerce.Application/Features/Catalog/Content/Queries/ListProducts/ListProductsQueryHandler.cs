using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Catalog.Content.DTOs;
using ECommerce.Application.Responses;
using ECommerce.Application.Common.Caching;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ECommerce.Application.Features.Catalog.Content.Queries.ListProducts;

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, PagedResult<ProductListItemDto>>
{
    private readonly IProductRepository  _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemoryCache        _cache;
    private readonly IProductCacheSignal _cacheSignal;

    public ListProductsQueryHandler(
        IProductRepository  productRepository,
        ICurrentUserService currentUserService,
        IMemoryCache        cache,
        IProductCacheSignal cacheSignal)
    {
        _productRepository  = productRepository;
        _currentUserService = currentUserService;
        _cache              = cache;
        _cacheSignal        = cacheSignal;
    }

    public async Task<PagedResult<ProductListItemDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException();

        var parameters  = request.Parameters;
        var pageNumber  = Math.Max(parameters.PageNumber, 1);
        var pageSize    = parameters.PageSize; // already capped by setter

        var cacheKey = $"products:list:user:{userId}:page:{parameters.PageNumber}:size:{parameters.PageSize}:search:{parameters.SearchQuery}:status:{parameters.Status}:min:{parameters.MinPrice}:max:{parameters.MaxPrice}:sort:{parameters.SortBy}:desc:{parameters.SortDescending}";

        if (!_cache.TryGetValue(cacheKey, out PagedResult<ProductListItemDto>? result) || result == null)
        {
            var (items, totalCount) = await _productRepository.GetPagedAsync(
                userId,
                parameters,
                cancellationToken);

            var dtos = items.Select(p => new ProductListItemDto
            {
                Id           = p.Id,
                Name         = p.Name,
                Description  = p.Description,
                Status       = p.Status.ToString(),
                BasePrice    = p.BasePrice,
                CreatedAt    = p.CreatedAt,
                UpdatedAt    = p.UpdatedAt,
                VariantCount = p.Variants?.Count ?? 0
            }).ToList();

            result = new PagedResult<ProductListItemDto>(dtos, totalCount, pageNumber, pageSize);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .AddExpirationToken(new CancellationChangeToken(_cacheSignal.Token));

            _cache.Set(cacheKey, result, cacheOptions);
        }

        return result;
    }
}
