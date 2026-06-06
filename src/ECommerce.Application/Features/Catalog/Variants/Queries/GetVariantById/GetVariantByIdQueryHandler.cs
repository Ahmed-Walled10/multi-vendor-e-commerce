using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Catalog.Variants.DTOs;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Variants.Queries.GetVariantById;

public class GetVariantByIdQueryHandler : IRequestHandler<GetVariantByIdQuery, VariantDto>
{
    private readonly IVariantRepository _variantRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetVariantByIdQueryHandler(
        IVariantRepository variantRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService)
    {
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
    }

    public async Task<VariantDto> Handle(GetVariantByIdQuery request, CancellationToken cancellationToken)
    {
        var variant = await _variantRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("ProductVariant", request.Id);

        if (variant.ProductId != request.ProductId)
            throw new NotFoundException("ProductVariant", request.Id);

        // Verify ownership
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        if (product.UserId != userId)
            throw new ForbiddenException("You do not own this product.");

        return new VariantDto
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            SKU = variant.SKU,
            Quantity = variant.Quantity,
            PriceOverride = variant.PriceOverride,
            IsActive = variant.IsActive,
            CreatedAt = variant.CreatedAt,
            UpdatedAt = variant.UpdatedAt,
            AttributeValues = variant.VariantAttributeValues?.Select(vav => new VariantAttributeValueDto
            {
                AttributeId = vav.AttributeValue.Attribute.Id,
                AttributeName = vav.AttributeValue.Attribute.Name,
                AttributeValueId = vav.AttributeValue.Id,
                Value = vav.AttributeValue.Value
            }).ToList() ?? new()
        };
    }
}
