using Corvel.ToDo.Abstractions.Requests;
using Corvel.ToDo.Common.Constants;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public class CreateToDoItemRequestValidator : AbstractValidator<CreateToDoItemRequest>
{
    public CreateToDoItemRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(ValidationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.DescriptionMaxLength)
            .When(x => x.Description is not null);

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.DueDate)
            .Must(dueDate => dueDate > timeProvider.GetUtcNow().UtcDateTime)
            .WithMessage("'Due Date' must be in the future.")
            .When(x => x.DueDate.HasValue);
    }
}
