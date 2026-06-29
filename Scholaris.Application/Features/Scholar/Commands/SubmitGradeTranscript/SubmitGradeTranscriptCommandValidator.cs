using FluentValidation;

public class SubmitGradeTranscriptCommandValidator : AbstractValidator<SubmitGradeTranscriptCommand>
{
    public SubmitGradeTranscriptCommandValidator()
    {
        RuleFor(c => c.ScholarUserId).NotEmpty();
        RuleFor(c => c.Grades).NotEmpty().WithMessage("There are no courses to report grades for.");
        RuleForEach(c => c.Grades).ChildRules(grade =>
        {
            grade.RuleFor(g => g.CourseCode).NotEmpty();
            grade.RuleFor(g => g.Units).GreaterThan(0);
            grade.RuleFor(g => g.Grade).GreaterThanOrEqualTo(0);
        });
    }
}
