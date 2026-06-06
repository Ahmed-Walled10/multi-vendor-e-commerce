using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Catalog.Variants.DTOs;
using ECommerce.Domain.Entities.Catalog;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Variants.Commands.CreateVariant;

public class CreateVariantCommandHandler : IRequestHandler<CreateVariantCommand, VariantDto>
{
    private readonly IVariantRepository _variantRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVariantCommandHandler(
        IVariantRepository variantRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<VariantDto> Handle(CreateVariantCommand request, CancellationToken cancellationToken)
    {
        // 1. Get product with attributes loaded
        var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        // 2. Verify ownership
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        if (product.UserId != userId)
            throw new ForbiddenException("You do not own this product.");

        // 3. Check SKU uniqueness
        if (await _variantRepository.SkuExistsAsync(request.SKU, cancellationToken: cancellationToken))
            throw new ConflictException($"A variant with SKU '{request.SKU}' already exists.");

        // 4. Validate that all attribute value IDs belong to this product
        var validAttributeValueIds = product.Attributes
            .SelectMany(a => a.Values)
            .Select(v => v.Id)
            .ToHashSet();

        foreach (var attrValueId in request.AttributeValueIds)
        {
            if (!validAttributeValueIds.Contains(attrValueId))
                throw new ECommerce.Domain.Exceptions.ValidationException(
                    [new FluentValidation.Results.ValidationFailure("AttributeValueIds",
                        $"Attribute value ID '{attrValueId}' does not belong to this product.")]);
        }

        // 5. Create variant
        var variant = new ProductVariant
        {
            ProductId = request.ProductId,
            SKU = request.SKU,
            Quantity = request.Quantity,
            PriceOverride = request.PriceOverride,
            IsActive = request.IsActive
        };

        // 6. Link attribute values via junction table
        foreach (var attrValueId in request.AttributeValueIds)
        {
            variant.VariantAttributeValues.Add(new ProductVariantAttributeValue
            {
                VariantId = variant.Id,
                AttributeValueId = attrValueId
            });
        }

        await _variantRepository.AddAsync(variant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Reload with details for the response
        var saved = await _variantRepository.GetByIdWithDetailsAsync(variant.Id, cancellationToken);
        return MapToDto(saved!);
    }

    private static VariantDto MapToDto(ProductVariant variant)
    {
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
