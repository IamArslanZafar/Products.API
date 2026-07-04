using FluentValidation;

namespace Products.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Colour).NotEmpty();
        RuleFor(p => p.Price).GreaterThanOrEqualTo(0);
    }
}
