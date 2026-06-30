using Microsoft.EntityFrameworkCore;

public class GetScholarshipsOverviewQueryHandler : IQueryHandler<GetScholarshipsOverviewQuery, List<ScholarshipOverviewDto>>
{
    private readonly IApplicationDbContext _context;
    public GetScholarshipsOverviewQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ScholarshipOverviewDto>> Handle(GetScholarshipsOverviewQuery request, CancellationToken cancellationToken)
    {
        var scholarships = await _context.Scholarships.AsNoTracking().ToListAsync(cancellationToken);
        var scholars = await _context.Scholars.AsNoTracking().ToListAsync(cancellationToken);

        return scholarships
            .OrderBy(s => s.Name)
            .Select(s => new ScholarshipOverviewDto(
                s.Id, s.Name, s.Description,
                s.Grants.Count,
                scholars.Count(sc => sc.ScholarshipId == s.Id)))
            .ToList();
    }
}
