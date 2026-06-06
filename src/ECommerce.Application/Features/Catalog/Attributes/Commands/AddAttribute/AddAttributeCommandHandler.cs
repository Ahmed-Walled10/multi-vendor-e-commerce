using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Entities.Catalog;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.AddAttribute;

public class AddAttributeCommandHandler : IRequestHandler<AddAttributeCommand, AttributeDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AddAttributeCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<AttributeDto> Handle(AddAttributeCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId, cancellationToken);
        if (product == null) throw new NotFoundException("Product", request.ProductId);
        if (product.UserId != _currentUserService.UserId) throw new ForbiddenException();

        if (product.Attributes.Any(a => a.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            throw new ConflictException($"Attribute '{request.Name}' already exists on this product.");

        var attribute = new ProductAttribute { Name = request.Name, ProductId = request.ProductId };
        await _productRepository.AddAttributeAsync(attribute, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AttributeDto { Id = attribute.Id, Name = attribute.Name };
    }
}
