using FluentValidation;

public class UpdateScholarshipCommandValidator : AbstractValidator<UpdateScholarshipCommand>
{
    public UpdateScholarshipCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(256);
    }
}
