using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.Products.Commands.CreateProduct;
using Products.Application.Products.Queries.GetProducts;

namespace Products.API.Controllers;

// Deliberately thin: no validation or persistence logic here — CreateProductCommand and
// GetProductsQuery are validated and handled entirely by the MediatR pipeline (see
// ValidationBehavior and their respective handlers in Products.Application).
[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? colour,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = GetProductsQuery.DefaultPageSize)
    {
        var query = GetProductsQuery.FromRequest(colour, minPrice, maxPrice, page, pageSize);
        var response = await _mediator.Send(query);
        return Ok(response);
    }
}
