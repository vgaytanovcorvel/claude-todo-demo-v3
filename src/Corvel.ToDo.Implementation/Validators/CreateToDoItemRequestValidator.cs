using Corvel.ToDo.Abstractions.Requests;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public class CreateToDoItemRequestValidator : AbstractValidator<CreateToDoItemRequest>
{
    public CreateToDoItemRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Title).ApplyTitleRules();

        RuleFor(x => x.Description)
            .ApplyDescriptionRules()
            .When(x => x.Description is not null);

        RuleFor(x => x.Priority).IsInEnum();

        RuleFor(x => x.DueDate)
            .Must(dueDate => dueDate > timeProvider.GetUtcNow().UtcDateTime)
            .WithMessage("'Due Date' must be in the future.")
            .When(x => x.DueDate.HasValue);
    }
}
