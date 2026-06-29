using Microsoft.EntityFrameworkCore;

public class GetScholarDashboardQueryHandler : IQueryHandler<GetScholarDashboardQuery, ScholarDashboardDto?>
{
    private readonly IApplicationDbContext _context;

    public GetScholarDashboardQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<ScholarDashboardDto?> Handle(GetScholarDashboardQuery request, CancellationToken cancellationToken)
    {
        var scholar = await _context.Scholars.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == request.ScholarUserId, cancellationToken);
        if (scholar is null)
            return null;

        var user = await _context.DomainUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == scholar.UserId, cancellationToken);
        var school = await _context.Schools.AsNoTracking().FirstOrDefaultAsync(s => s.Id == scholar.SchoolId, cancellationToken);
        var scholarship = await _context.Scholarships.AsNoTracking().FirstOrDefaultAsync(s => s.Id == scholar.ScholarshipId, cancellationToken);

        // Compliance for the current open term.
        var compliance = ComplianceState.NoOpenTerm;
        int? openTermNumber = null;
        var openTerm = school?.Terms.FirstOrDefault(t => t.IsOpen);
        if (openTerm is not null)
        {
            openTermNumber = openTerm.TermNumber;
            var record = await _context.TermRecords.AsNoTracking()
                .FirstOrDefaultAsync(r => r.ScholarId == scholar.Id && r.TermId == openTerm.Id, cancellationToken);
            compliance = record?.ProofOfRegistration is not null ? ComplianceState.Submitted : ComplianceState.NeedsSubmission;
        }

        // Latest stipend announcement for the scholar's region.
        DashboardStipendDto? latestStipend = null;
        if (school is not null)
        {
            var drops = await _context.StipendDrops.AsNoTracking()
                .Where(d => d.Region == school.Region)
                .ToListAsync(cancellationToken);
            var latest = drops.OrderByDescending(d => d.AnnouncedAt).FirstOrDefault();
            if (latest is not null)
            {
                var receipt = latest.Receipts.FirstOrDefault(r => r.ScholarId == scholar.Id);
                latestStipend = new DashboardStipendDto(latest.Id, latest.Amount, latest.Description, latest.AnnouncedAt, receipt?.Received);
            }
        }

        int? age = null;
        if (user?.Profile?.DateOfBirth is { } dob)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            age = today.Year - dob.Year;
            if (dob > today.AddYears(-age.Value)) age--;
        }

        return new ScholarDashboardDto(
            UserDisplay.NameOf(user),
            age,
            school?.Name ?? string.Empty,
            scholar.DegreeProgram,
            scholar.BatchNumber,
            scholarship?.Name ?? string.Empty,
            user?.Email ?? string.Empty,
            scholar.CurrentStatus.Status,
            compliance,
            openTermNumber,
            latestStipend);
    }
}
