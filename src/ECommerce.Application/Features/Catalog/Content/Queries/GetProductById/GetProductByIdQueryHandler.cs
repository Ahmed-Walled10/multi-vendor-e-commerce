using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Catalog.Content.DTOs;
using ECommerce.Application.Common.Caching;
using ECommerce.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ECommerce.Application.Features.Catalog.Content.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemoryCache _cache;
    private readonly IProductCacheSignal _cacheSignal;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        ICurrentUserService currentUserService,
        IMemoryCache cache,
        IProductCacheSignal cacheSignal)
    {
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _cache = cache;
        _cacheSignal = cacheSignal;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var cacheKey = $"products:detail:id:{request.Id}:user:{userId}";

        if (!_cache.TryGetValue(cacheKey, out ProductDto? result) || result == null)
        {
            var product = await _productRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Product", request.Id);

            if (product.UserId != userId)
                throw new ForbiddenException("You do not own this product.");

            result = new ProductDto
            {
                Id = product.Id,
                UserId = product.UserId,
                Name = product.Name,
                Description = product.Description,
                Status = product.Status.ToString(),
                BasePrice = product.BasePrice,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                VariantCount = product.Variants?.Count ?? 0,
                Attributes = product.Attributes?.Select(a => new ProductAttributeDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Values = a.Values?.Select(v => new ProductAttributeValueDto
                    {
                        Id = v.Id,
                        Value = v.Value
                    }).ToList() ?? new()
                }).ToList() ?? new()
            };

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .AddExpirationToken(new CancellationChangeToken(_cacheSignal.Token));

            _cache.Set(cacheKey, result, cacheOptions);
        }

        return result;
    }
}
