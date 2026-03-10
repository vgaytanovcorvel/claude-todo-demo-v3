using Corvel.ToDo.Abstractions.Requests;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName).ApplyNameRules();
        RuleFor(x => x.LastName).ApplyNameRules();
        RuleFor(x => x.Email).ApplyEmailRules();
    }
}
