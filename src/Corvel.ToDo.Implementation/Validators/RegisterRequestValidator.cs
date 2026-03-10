using Corvel.ToDo.Abstractions.Requests;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).ApplyEmailRules();
        RuleFor(x => x.Password).ApplyPasswordRules();
        RuleFor(x => x.FirstName).ApplyNameRules();
        RuleFor(x => x.LastName).ApplyNameRules();
    }
}
