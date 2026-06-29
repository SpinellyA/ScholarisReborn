using FluentValidation;

public class AddScholarshipGrantCommandValidator : AbstractValidator<AddScholarshipGrantCommand>
{
    public AddScholarshipGrantCommandValidator()
    {
        RuleFor(c => c.ScholarshipId).NotEmpty();
        RuleFor(c => c.GrantName).NotEmpty().MaximumLength(256);
        RuleFor(c => c.Amount).GreaterThan(0);
    }
}
