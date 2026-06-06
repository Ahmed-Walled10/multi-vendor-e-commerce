using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.UpdateAttributeValue;

public class UpdateAttributeValueCommandHandler : IRequestHandler<UpdateAttributeValueCommand, AttributeValueDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAttributeValueCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<AttributeValueDto> Handle(UpdateAttributeValueCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId, cancellationToken);
        if (product == null) throw new NotFoundException("Product", request.ProductId);
        if (product.UserId != _currentUserService.UserId) throw new ForbiddenException();

        var attribute = product.Attributes.FirstOrDefault(a => a.Id == request.AttributeId);
        if (attribute == null) throw new NotFoundException("Attribute", request.AttributeId);

        var attrValue = attribute.Values.FirstOrDefault(v => v.Id == request.ValueId);
        if (attrValue == null) throw new NotFoundException("AttributeValue", request.ValueId);

        if (attribute.Values.Any(v => v.Id != request.ValueId && v.Value.Equals(request.Value, StringComparison.OrdinalIgnoreCase)))
            throw new ConflictException($"Another value identical to '{request.Value}' already exists for this attribute.");

        attrValue.Value = request.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AttributeValueDto { Id = attrValue.Id, Value = attrValue.Value };
    }
}
