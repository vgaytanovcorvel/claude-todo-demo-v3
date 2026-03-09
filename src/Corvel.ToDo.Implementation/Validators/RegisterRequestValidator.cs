using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(ValidationConstants.EmailMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(ValidationConstants.PasswordMinLength)
            .MaximumLength(ValidationConstants.PasswordMaxLength);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(ValidationConstants.NameMaxLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(ValidationConstants.NameMaxLength);
    }
}
