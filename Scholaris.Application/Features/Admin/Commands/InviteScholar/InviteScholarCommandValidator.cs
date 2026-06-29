using FluentValidation;
using Microsoft.EntityFrameworkCore;

public class InviteScholarCommandValidator : AbstractValidator<InviteScholarCommand>
{
    private readonly IApplicationDbContext _context;

    public InviteScholarCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(c => c.invitedByAdminId).NotEmpty();
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.schoolId).NotEmpty();
        RuleFor(c => c.scholarshipId).NotEmpty();

        RuleFor(c => c.schoolId)
            .MustAsync(SchoolExistsAsync)
            .WithMessage("The selected school does not exist.")
            .When(c => c.schoolId != Guid.Empty);

        RuleFor(c => c.scholarshipId)
            .MustAsync(ScholarshipExistsAsync)
            .WithMessage("The selected scholarship does not exist.")
            .When(c => c.scholarshipId != Guid.Empty);
    }

    private Task<bool> SchoolExistsAsync(Guid schoolId, CancellationToken cancellationToken)
        => _context.Schools.AnyAsync(s => s.Id == schoolId, cancellationToken);

    private Task<bool> ScholarshipExistsAsync(Guid scholarshipId, CancellationToken cancellationToken)
        => _context.Scholarships.AnyAsync(s => s.Id == scholarshipId, cancellationToken);
}
