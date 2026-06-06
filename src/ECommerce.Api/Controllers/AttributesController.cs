using ECommerce.Application.Features.Catalog.Attributes.Commands.AddAttribute;
using ECommerce.Application.Features.Catalog.Attributes.Commands.UpdateAttribute;
using ECommerce.Application.Features.Catalog.Attributes.Commands.DeleteAttribute;
using ECommerce.Application.Features.Catalog.Attributes.Commands.AddAttributeValue;
using ECommerce.Application.Features.Catalog.Attributes.Commands.UpdateAttributeValue;
using ECommerce.Application.Features.Catalog.Attributes.Commands.DeleteAttributeValue;
using ECommerce.Application.Features.Catalog.Attributes.Queries.ListAttributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/Catalog/{productId:guid}/Attributes")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Produces("application/json")]
public class AttributesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttributesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddAttribute(Guid productId, [FromBody] AddAttributeCommand command)
    {
        command.ProductId = productId;
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(ListAttributes), new { productId }, new { message = "Attribute added successfully.", data = result });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListAttributes(Guid productId)
    {
        var result = await _mediator.Send(new ListAttributesQuery { ProductId = productId });
        return Ok(new { message = "Attributes retrieved successfully.", data = result });
    }

    [HttpPatch("{attributeId:guid}")]
    public async Task<IActionResult> UpdateAttribute(Guid productId, Guid attributeId, [FromBody] UpdateAttributeCommand command)
    {
        command.ProductId = productId;
        command.AttributeId = attributeId;
        var result = await _mediator.Send(command);
        return Ok(new { message = "Attribute updated successfully.", data = result });
    }

    [HttpDelete("{attributeId:guid}")]
    public async Task<IActionResult> DeleteAttribute(Guid productId, Guid attributeId)
    {
        await _mediator.Send(new DeleteAttributeCommand { ProductId = productId, AttributeId = attributeId });
        return Ok(new { message = "Attribute deleted successfully." });
    }

    [HttpPost("{attributeId:guid}/Values")]
    public async Task<IActionResult> AddAttributeValue(Guid productId, Guid attributeId, [FromBody] AddAttributeValueCommand command)
    {
        command.ProductId = productId;
        command.AttributeId = attributeId;
        var result = await _mediator.Send(command);
        return Ok(new { message = "Attribute value added successfully.", data = result });
    }

    [HttpPatch("{attributeId:guid}/Values/{valueId:guid}")]
    public async Task<IActionResult> UpdateAttributeValue(Guid productId, Guid attributeId, Guid valueId, [FromBody] UpdateAttributeValueCommand command)
    {
        command.ProductId = productId;
        command.AttributeId = attributeId;
        command.ValueId = valueId;
        var result = await _mediator.Send(command);
        return Ok(new { message = "Attribute value updated successfully.", data = result });
    }

    [HttpDelete("{attributeId:guid}/Values/{valueId:guid}")]
    public async Task<IActionResult> DeleteAttributeValue(Guid productId, Guid attributeId, Guid valueId)
    {
        await _mediator.Send(new DeleteAttributeValueCommand { ProductId = productId, AttributeId = attributeId, ValueId = valueId });
        return Ok(new { message = "Attribute value deleted successfully." });
    }
}
