using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Catalog.Content.DTOs;
using ECommerce.Application.Common.Caching;
using ECommerce.Domain.Entities.Catalog;
using ECommerce.Domain.Common.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Catalog.Content.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    private readonly IProductCacheSignal _cacheSignal;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger,
        IProductCacheSignal cacheSignal)
    {
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cacheSignal = cacheSignal;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🚀 Creating product: '{ProductName}'", request.Name);

        // 1. Get current user
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            _logger.LogWarning("⚠️ Unauthorized product creation attempt for name: '{ProductName}'", request.Name);
            throw new UnauthorizedAccessException();
        }
        
        // 2. Parse status
        var status = ProductStatus.Draft;
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ProductStatus>(request.Status, true, out var parsed))
            status = parsed;

        // 3. Create product
        var product = new Product
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            BasePrice = request.BasePrice,
            Status = status
        };

        // 4. Add attributes if provided
        if (request.Attributes != null)
        {
            foreach (var attrDto in request.Attributes)
            {
                var attribute = new ProductAttribute
                {
                    ProductId = product.Id,
                    Name = attrDto.Name
                };

                foreach (var value in attrDto.Values)
                {
                    attribute.Values.Add(new ProductAttributeValue
                    {
                        AttributeId = attribute.Id,
                        Value = value
                    });
                }

                product.Attributes.Add(attribute);
            }
        }

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Product created successfully. ID: {ProductId} for User: {UserId}", product.Id, userId);

        _cacheSignal.Reset();

        // 5. Map to DTO
        return MapToDto(product);
    }

    private static ProductDto MapToDto(Product product)
    {
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
