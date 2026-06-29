using FluentValidation;

public class OpenTermCommandValidator : AbstractValidator<OpenTermCommand>
{
    public OpenTermCommandValidator()
    {
        RuleFor(c => c.SchoolId).NotEmpty();
        RuleFor(c => c.TermNumber).GreaterThan(0);
    }
}
