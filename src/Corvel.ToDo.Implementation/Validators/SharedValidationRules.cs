using Corvel.ToDo.Common.Constants;
using FluentValidation;

namespace Corvel.ToDo.Implementation.Validators;

public static class SharedValidationRules
{
    public static IRuleBuilderOptions<T, string> ApplyTitleRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(ValidationConstants.TitleMaxLength);
    }

    public static IRuleBuilderOptions<T, string?> ApplyDescriptionRules<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(ValidationConstants.DescriptionMaxLength);
    }

    public static IRuleBuilderOptions<T, TEnum> ApplyEnumRules<T, TEnum>(this IRuleBuilder<T, TEnum> ruleBuilder)
        where TEnum : struct, Enum
    {
        return ruleBuilder.IsInEnum();
    }

    public static void ApplyDueDateRules<T>(
        this IRuleBuilder<T, DateTime?> ruleBuilder,
        TimeProvider timeProvider)
    {
        ruleBuilder
            .Must(dueDate => dueDate > timeProvider.GetUtcNow().UtcDateTime)
            .WithMessage("'Due Date' must be in the future.")
            .When(x => true);
    }

    public static IRuleBuilderOptions<T, string> ApplyEmailRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(ValidationConstants.EmailMaxLength);
    }

    public static IRuleBuilderOptions<T, string> ApplyNameRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(ValidationConstants.NameMaxLength);
    }

    public static IRuleBuilderOptions<T, string> ApplyPasswordRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(ValidationConstants.PasswordMinLength)
            .MaximumLength(ValidationConstants.PasswordMaxLength);
    }
}
