using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(ValidationConstants.EmailMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(ValidationConstants.PasswordMaxLength);
    }
}
