using Microsoft.EntityFrameworkCore;

public class GetSchoolSubmissionStatsQueryHandler : IQueryHandler<GetSchoolSubmissionStatsQuery, List<SchoolSubmissionStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSchoolSubmissionStatsQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<List<SchoolSubmissionStatsDto>> Handle(GetSchoolSubmissionStatsQuery request, CancellationToken cancellationToken)
    {
        var schools = await _context.Schools.AsNoTracking().ToListAsync(cancellationToken);
        var scholars = await _context.Scholars.AsNoTracking().ToListAsync(cancellationToken);
        var records = await _context.TermRecords.AsNoTracking().ToListAsync(cancellationToken);

        return schools
            .OrderBy(s => s.Name)
            .Select(school =>
            {
                var schoolScholars = scholars.Where(s => s.SchoolId == school.Id).ToList();
                var scholarIds = schoolScholars.Select(s => s.Id).ToHashSet();
                var active = schoolScholars.Count(s => s.CurrentStatus.Status == ScholasticStatus.Active);
                var pending = records.Count(r => scholarIds.Contains(r.ScholarId) && r.Status == RecordStatus.Pending);
                return new SchoolSubmissionStatsDto(school.Id, school.Name, school.Region, active, pending);
            })
            .ToList();
    }
}
