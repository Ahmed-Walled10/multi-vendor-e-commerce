using MediatR;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.DeleteAttribute;

public class DeleteAttributeCommandHandler : IRequestHandler<DeleteAttributeCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAttributeCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteAttributeCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId, cancellationToken);
        if (product == null) throw new NotFoundException("Product", request.ProductId);
        if (product.UserId != _currentUserService.UserId) throw new ForbiddenException();

        var attribute = product.Attributes.FirstOrDefault(a => a.Id == request.AttributeId);
        if (attribute == null) throw new NotFoundException("Attribute", request.AttributeId);

        product.Attributes.Remove(attribute);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
