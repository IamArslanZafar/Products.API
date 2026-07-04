using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.API.Requests;
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
    public async Task<IActionResult> GetAll([FromQuery] GetProductsRequest request)
    {
        var query = GetProductsQuery.FromRequest(request.Colour, request.MinPrice, request.MaxPrice, request.Page, request.PageSize);
        var response = await _mediator.Send(query);
        return Ok(response);
    }
}
