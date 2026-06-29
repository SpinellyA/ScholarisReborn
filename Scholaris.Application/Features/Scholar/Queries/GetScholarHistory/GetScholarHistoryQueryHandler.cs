using Microsoft.EntityFrameworkCore;

public class GetScholarHistoryQueryHandler : IQueryHandler<GetScholarHistoryQuery, ScholarHistoryDto>
{
    private readonly IApplicationDbContext _context;

    public GetScholarHistoryQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<ScholarHistoryDto> Handle(GetScholarHistoryQuery request, CancellationToken cancellationToken)
    {
        var empty = new ScholarHistoryDto(new(), new(), new());

        var scholar = await _context.Scholars.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == request.ScholarUserId, cancellationToken);
        if (scholar is null)
            return empty;

        var school = await _context.Schools.AsNoTracking().FirstOrDefaultAsync(s => s.Id == scholar.SchoolId, cancellationToken);
        var termNumberById = school?.Terms.ToDictionary(t => t.Id, t => t.TermNumber) ?? new();

        var records = await _context.TermRecords.AsNoTracking()
            .Where(r => r.ScholarId == scholar.Id)
            .ToListAsync(cancellationToken);

        var compliance = records
            .Select(r => new ComplianceHistoryItem(
                termNumberById.GetValueOrDefault(r.TermId),
                r.GradeTranscript?.GWA,
                r.Status,
                r.ProofOfRegistration is not null,
                r.GradeTranscript is not null))
            .OrderByDescending(c => c.TermNumber)
            .ToList();

        var statuses = scholar.Statuses
            .Select(s => new StatusHistoryItem(s.Status, s.StartTime, s.EndTime, s.Reason))
            .OrderByDescending(s => s.StartTime)
            .ToList();

        var stipends = new List<StipendHistoryItem>();
        if (school is not null)
        {
            var drops = await _context.StipendDrops.AsNoTracking()
                .Where(d => d.Region == school.Region)
                .ToListAsync(cancellationToken);
            stipends = drops
                .OrderByDescending(d => d.AnnouncedAt)
                .Select(d => new StipendHistoryItem(
                    d.Amount, d.Description, d.AnnouncedAt,
                    d.Receipts.FirstOrDefault(r => r.ScholarId == scholar.Id)?.Received))
                .ToList();
        }

        return new ScholarHistoryDto(compliance, statuses, stipends);
    }
}
