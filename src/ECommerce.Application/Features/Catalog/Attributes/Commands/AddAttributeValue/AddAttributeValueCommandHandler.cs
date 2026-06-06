using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Entities.Catalog;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.AddAttributeValue;

public class AddAttributeValueCommandHandler : IRequestHandler<AddAttributeValueCommand, AttributeValueDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AddAttributeValueCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<AttributeValueDto> Handle(AddAttributeValueCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId, cancellationToken);
        if (product == null) throw new NotFoundException("Product", request.ProductId);
        if (product.UserId != _currentUserService.UserId) throw new ForbiddenException();

        var attribute = product.Attributes.FirstOrDefault(a => a.Id == request.AttributeId);
        if (attribute == null) throw new NotFoundException("Attribute", request.AttributeId);

        if (attribute.Values.Any(v => v.Value.Equals(request.Value, StringComparison.OrdinalIgnoreCase)))
            throw new ConflictException($"Value '{request.Value}' already exists for this attribute.");

        var attrValue = new ProductAttributeValue { Value = request.Value, AttributeId = request.AttributeId };
        await _productRepository.AddAttributeValueAsync(attrValue, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AttributeValueDto { Id = attrValue.Id, Value = attrValue.Value };
    }
}
