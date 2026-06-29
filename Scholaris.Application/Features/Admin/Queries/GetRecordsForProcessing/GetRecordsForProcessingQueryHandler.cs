using Microsoft.EntityFrameworkCore;

public class GetRecordsForProcessingQueryHandler : IQueryHandler<GetRecordsForProcessingQuery, List<RecordListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRecordsForProcessingQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<List<RecordListItemDto>> Handle(GetRecordsForProcessingQuery request, CancellationToken cancellationToken)
    {
        var scholars = await _context.Scholars.AsNoTracking()
            .Where(s => s.SchoolId == request.SchoolId && s.BatchNumber == request.BatchNumber)
            .ToListAsync(cancellationToken);
        if (scholars.Count == 0)
            return new();

        var scholarIds = scholars.Select(s => s.Id).ToHashSet();

        var school = await _context.Schools.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.SchoolId, cancellationToken);
        var termNumberById = school?.Terms.ToDictionary(t => t.Id, t => t.TermNumber) ?? new();

        var records = await _context.TermRecords.AsNoTracking()
            .Where(r => scholarIds.Contains(r.ScholarId))
            .ToListAsync(cancellationToken);

        // Resolve names for both the scholars (via their UserId) and the processing admins.
        var userIds = scholars.Select(s => s.UserId)
            .Concat(records.Where(r => r.ProcessedByAdminId.HasValue).Select(r => r.ProcessedByAdminId!.Value))
            .Distinct().ToList();
        var users = await _context.DomainUsers.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToListAsync(cancellationToken);
        var userById = users.ToDictionary(u => u.Id);

        return records
            .Select(r =>
            {
                var scholar = scholars.First(s => s.Id == r.ScholarId);
                userById.TryGetValue(scholar.UserId, out var scholarUser);
                User? processor = null;
                if (r.ProcessedByAdminId.HasValue) userById.TryGetValue(r.ProcessedByAdminId.Value, out processor);

                return new RecordListItemDto(
                    r.Id,
                    UserDisplay.NameOf(scholarUser),
                    scholar.DegreeProgram,
                    scholar.BatchNumber,
                    termNumberById.GetValueOrDefault(r.TermId),
                    r.Status,
                    r.GradeTranscript?.GWA,
                    r.ProofOfRegistration is not null,
                    r.GradeTranscript is not null,
                    r.ProcessedByAdminId.HasValue ? UserDisplay.NameOf(processor) : null);
            })
            .OrderByDescending(r => r.TermNumber)
            .ThenBy(r => r.ScholarName)
            .ToList();
    }
}
