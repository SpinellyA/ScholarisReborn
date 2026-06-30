using FluentValidation;

public class UpdateSchoolCommandValidator : AbstractValidator<UpdateSchoolCommand>
{
    public UpdateSchoolCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.SchoolCode).NotEmpty().MaximumLength(32);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(256);
    }
}
