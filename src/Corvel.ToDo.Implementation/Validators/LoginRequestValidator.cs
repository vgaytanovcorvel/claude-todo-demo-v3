using Corvel.ToDo.Abstractions.Requests;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
