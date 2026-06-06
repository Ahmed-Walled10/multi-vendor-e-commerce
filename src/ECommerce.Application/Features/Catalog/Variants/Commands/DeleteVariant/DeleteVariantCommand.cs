using MediatR;

namespace ECommerce.Application.Features.Catalog.Variants.Commands.DeleteVariant;

public class DeleteVariantCommand : IRequest<Unit>
{
    public Guid ProductId { get; set; }
    public Guid Id { get; set; }
}
