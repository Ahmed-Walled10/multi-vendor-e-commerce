using MediatR;

namespace ECommerce.Application.Features.Catalog.Content.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
