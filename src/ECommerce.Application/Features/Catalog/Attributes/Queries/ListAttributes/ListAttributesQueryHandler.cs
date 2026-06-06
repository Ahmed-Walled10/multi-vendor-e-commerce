using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Features.Catalog.Attributes.Queries.ListAttributes;

public class ListAttributesQueryHandler : IRequestHandler<ListAttributesQuery, List<AttributeDto>>
{
    private readonly IProductRepository _productRepository;

    public ListAttributesQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<AttributeDto>> Handle(ListAttributesQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId, cancellationToken);
        if (product == null) throw new NotFoundException("Product", request.ProductId);

        return product.Attributes.Select(a => new AttributeDto
        {
            Id = a.Id,
            Name = a.Name,
            Values = a.Values.Select(v => new AttributeValueDto { Id = v.Id, Value = v.Value }).ToList()
        }).ToList();
    }
}
