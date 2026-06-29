using Microsoft.EntityFrameworkCore;

public class GetSubmissionContextQueryHandler : IQueryHandler<GetSubmissionContextQuery, SubmissionContextDto>
{
    private readonly IApplicationDbContext _context;

    public GetSubmissionContextQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<SubmissionContextDto> Handle(GetSubmissionContextQuery request, CancellationToken cancellationToken)
    {
        var empty = new SubmissionContextDto(false, 0, string.Empty, false, false, null, new());

        var scholar = await _context.Scholars.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == request.ScholarUserId, cancellationToken);
        if (scholar is null)
            return empty;

        var school = await _context.Schools.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == scholar.SchoolId, cancellationToken);
        if (school is null)
            return empty;

        var openTerm = school.Terms.FirstOrDefault(t => t.IsOpen);
        if (openTerm is null)
            return new SubmissionContextDto(false, 0, school.Name, false, false, null, new());

        var records = await _context.TermRecords.AsNoTracking()
            .Where(r => r.ScholarId == scholar.Id)
            .ToListAsync(cancellationToken);

        var current = records.FirstOrDefault(r => r.TermId == openTerm.Id);

        // Prior-term courses to report grades for: most recent earlier term that has a PoR.
        var priorCourses = new List<PriorCourseDto>();
        foreach (var term in school.Terms.Where(t => t.TermNumber < openTerm.TermNumber).OrderByDescending(t => t.TermNumber))
        {
            var rec = records.FirstOrDefault(r => r.TermId == term.Id
                && r.ProofOfRegistration is not null && r.ProofOfRegistration.Courses.Count > 0);
            if (rec is not null)
            {
                priorCourses = rec.ProofOfRegistration!.Courses
                    .Select(c => new PriorCourseDto(c.CourseCode, c.Units))
                    .ToList();
                break;
            }
        }

        return new SubmissionContextDto(
            true,
            openTerm.TermNumber,
            school.Name,
            current?.ProofOfRegistration is not null,
            current?.GradeTranscript is not null,
            current?.Status,
            priorCourses);
    }
}
