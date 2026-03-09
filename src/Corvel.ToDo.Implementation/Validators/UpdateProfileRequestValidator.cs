using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(ValidationConstants.NameMaxLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(ValidationConstants.NameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(ValidationConstants.EmailMaxLength);
    }
}
