using FluentValidation;

public class CloseTermCommandValidator : AbstractValidator<CloseTermCommand>
{
    public CloseTermCommandValidator()
    {
        RuleFor(c => c.SchoolId).NotEmpty();
        RuleFor(c => c.TermId).NotEmpty();
    }
}
