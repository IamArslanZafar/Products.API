using FluentValidation;

namespace Products.Application.Auth.Queries.Login;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(l => l.Email).NotEmpty().EmailAddress();
        RuleFor(l => l.Password).NotEmpty();
    }
}
