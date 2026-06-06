using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Catalog.Variants.DTOs;
using ECommerce.Application.Responses;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Variants.Queries.ListVariants;

public class ListVariantsQueryHandler : IRequestHandler<ListVariantsQuery, PagedResult<VariantDto>>
{
    private readonly IVariantRepository _variantRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;

    public ListVariantsQueryHandler(
        IVariantRepository variantRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService)
    {
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<VariantDto>> Handle(ListVariantsQuery request, CancellationToken cancellationToken)
    {
        // Verify ownership
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        if (product.UserId != userId)
            throw new ForbiddenException("You do not own this product.");

        var (items, totalCount) = await _variantRepository.GetPagedAsync(
            request.ProductId,
            request.Parameters,
            cancellationToken);

        var dtos = items.Select(v => new VariantDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            SKU = v.SKU,
            Quantity = v.Quantity,
            PriceOverride = v.PriceOverride,
            IsActive = v.IsActive,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt,
            AttributeValues = v.VariantAttributeValues?.Select(vav => new VariantAttributeValueDto
            {
                AttributeId = vav.AttributeValue.Attribute.Id,
                AttributeName = vav.AttributeValue.Attribute.Name,
                AttributeValueId = vav.AttributeValue.Id,
                Value = vav.AttributeValue.Value
            }).ToList() ?? new()
        }).ToList();

        return new PagedResult<VariantDto>(dtos, totalCount, request.Parameters.PageNumber, request.Parameters.PageSize);
    }
}
