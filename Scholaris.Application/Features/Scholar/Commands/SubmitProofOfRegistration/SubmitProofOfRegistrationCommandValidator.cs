using FluentValidation;

public class SubmitProofOfRegistrationCommandValidator : AbstractValidator<SubmitProofOfRegistrationCommand>
{
    public SubmitProofOfRegistrationCommandValidator()
    {
        RuleFor(c => c.ScholarUserId).NotEmpty();
        RuleFor(c => c.FileName).NotEmpty();
        RuleFor(c => c.FileContent).NotEmpty().WithMessage("A registration document is required.");
        RuleFor(c => c.Courses).NotEmpty().WithMessage("Add at least one course.");
        RuleForEach(c => c.Courses).ChildRules(course =>
        {
            course.RuleFor(c => c.CourseCode).NotEmpty();
            course.RuleFor(c => c.Units).GreaterThan(0);
        });
    }
}
