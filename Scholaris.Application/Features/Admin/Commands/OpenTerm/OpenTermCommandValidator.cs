using FluentValidation;

public class OpenTermCommandValidator : AbstractValidator<OpenTermCommand>
{
    public OpenTermCommandValidator()
    {
        RuleFor(c => c.SchoolId).NotEmpty();
        RuleFor(c => c.AcademicYearStart).GreaterThan(2000);
        RuleFor(c => c.PeriodNumber).GreaterThan(0);
    }
}
