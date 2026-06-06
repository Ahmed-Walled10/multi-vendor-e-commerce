using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Common.Caching;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Content.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCacheSignal _cacheSignal;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IProductCacheSignal cacheSignal)
    {
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _cacheSignal = cacheSignal;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // 1. Get product
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Product", request.Id);

        // 2. Verify ownership
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        if (product.UserId != userId)
            throw new ForbiddenException("You do not own this product.");

        _productRepository.Delete(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cacheSignal.Reset();

        return Unit.Value;
    }
}
