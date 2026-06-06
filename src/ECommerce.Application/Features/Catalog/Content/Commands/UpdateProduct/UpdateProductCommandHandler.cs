using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Catalog.Content.DTOs;
using ECommerce.Application.Common.Caching;
using ECommerce.Domain.Common.Enums;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Content.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCacheSignal _cacheSignal;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IProductCacheSignal cacheSignal)
    {
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _cacheSignal = cacheSignal;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // 1. Get product with details
        var product = await _productRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Product", request.Id);

        // 2. Verify ownership
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        if (product.UserId != userId)
            throw new ForbiddenException("You do not own this product.");

        // 3. Update fields
        product.Name = request.Name;
        product.Description = request.Description;
        product.BasePrice = request.BasePrice;
        product.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ProductStatus>(request.Status, true, out var parsed))
            product.Status = parsed;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cacheSignal.Reset();

        // 4. Return DTO
        return new ProductDto
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
    }
}
