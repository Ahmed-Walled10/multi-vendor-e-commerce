using ECommerce.Application.Features.Catalog.Variants.Commands.CreateVariant;
using ECommerce.Application.Features.Catalog.Variants.Commands.DeleteVariant;
using ECommerce.Application.Features.Catalog.Variants.Commands.UpdateVariant;
using ECommerce.Application.Features.Catalog.Variants.DTOs;
using ECommerce.Application.Features.Catalog.Variants.Queries.GetVariantById;
using ECommerce.Application.Features.Catalog.Variants.Queries.ListVariants;
using ECommerce.Application.ResourceParameters;
using ECommerce.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/Catalog/{productId:guid}/Variants")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Produces("application/json")]
public class VariantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public VariantsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    [ProducesResponseType(typeof(VariantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(Guid productId, [FromBody] CreateVariantCommand command)
    {
        command.ProductId = productId;
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { productId, id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VariantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid productId, Guid id)
    {
        var result = await _mediator.Send(new GetVariantByIdQuery { ProductId = productId, Id = id });
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<VariantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(
        Guid productId,
        [FromQuery] VariantResourceParameters parameters)
    {
        var query = new ListVariantsQuery
        {
            ProductId = productId,
            Parameters = parameters
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

   
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VariantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid productId, Guid id, [FromBody] UpdateVariantCommand command)
    {
        command.ProductId = productId;
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid productId, Guid id)
    {
        await _mediator.Send(new DeleteVariantCommand { ProductId = productId, Id = id });
        return NoContent();
    }
}
