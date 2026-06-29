using FluentValidation;

public class CreateScholarshipCommandValidator : AbstractValidator<CreateScholarshipCommand>
{
    public CreateScholarshipCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(256);
    }
}
