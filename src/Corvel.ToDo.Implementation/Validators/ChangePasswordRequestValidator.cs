using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(ValidationConstants.PasswordMinLength)
            .MaximumLength(ValidationConstants.PasswordMaxLength);
    }
}
