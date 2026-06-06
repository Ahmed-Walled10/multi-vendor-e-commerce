using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.UpdateAttribute;

public class UpdateAttributeCommandHandler : IRequestHandler<UpdateAttributeCommand, AttributeDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAttributeCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<AttributeDto> Handle(UpdateAttributeCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId, cancellationToken);
        if (product == null) throw new NotFoundException("Product", request.ProductId);
        if (product.UserId != _currentUserService.UserId) throw new ForbiddenException();

        var attribute = product.Attributes.FirstOrDefault(a => a.Id == request.AttributeId);
        if (attribute == null) throw new NotFoundException("Attribute", request.AttributeId);

        if (product.Attributes.Any(a => a.Id != request.AttributeId && a.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            throw new ConflictException($"Another attribute with the name '{request.Name}' already exists.");

        attribute.Name = request.Name;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AttributeDto 
        { 
            Id = attribute.Id, 
            Name = attribute.Name, 
            Values = attribute.Values.Select(v => new AttributeValueDto { Id = v.Id, Value = v.Value }).ToList() 
        };
    }
}
