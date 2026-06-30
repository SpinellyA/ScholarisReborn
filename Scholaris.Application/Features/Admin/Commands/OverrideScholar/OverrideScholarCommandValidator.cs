using FluentValidation;

public class OverrideScholarCommandValidator : AbstractValidator<OverrideScholarCommand>
{
    public OverrideScholarCommandValidator()
    {
        RuleFor(c => c.ScholarId).NotEmpty();
        RuleFor(c => c.SchoolId).NotEmpty();
        RuleFor(c => c.ScholarshipId).NotEmpty();
        RuleFor(c => c.BatchNumber).GreaterThan(0);
        RuleFor(c => c.DegreeProgram).NotEmpty();
        RuleFor(c => c.FirstName).NotEmpty();
        RuleFor(c => c.LastName).NotEmpty();
    }
}
