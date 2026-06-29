using FluentValidation;

public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolCommandValidator()
    {
        RuleFor(c => c.SchoolCode).NotEmpty().MaximumLength(32);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(256);
    }
}
