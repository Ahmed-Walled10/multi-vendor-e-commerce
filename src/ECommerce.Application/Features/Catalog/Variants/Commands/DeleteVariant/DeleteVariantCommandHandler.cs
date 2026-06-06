using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Variants.Commands.DeleteVariant;

public class DeleteVariantCommandHandler : IRequestHandler<DeleteVariantCommand, Unit>
{
    private readonly IVariantRepository _variantRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVariantCommandHandler(
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

    public async Task<Unit> Handle(DeleteVariantCommand request, CancellationToken cancellationToken)
    {
        // 1. Get variant
        var variant = await _variantRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("ProductVariant", request.Id);

        if (variant.ProductId != request.ProductId)
            throw new NotFoundException("ProductVariant", request.Id);

        // 2. Verify ownership via product
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        if (product.UserId != userId)
            throw new ForbiddenException("You do not own this product.");

        // 3. Delete
        _variantRepository.Delete(variant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
