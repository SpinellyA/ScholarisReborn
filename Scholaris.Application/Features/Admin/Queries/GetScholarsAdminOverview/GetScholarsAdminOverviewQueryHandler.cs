using Microsoft.EntityFrameworkCore;

public class GetScholarsAdminOverviewQueryHandler : IQueryHandler<GetScholarsAdminOverviewQuery, List<ScholarAdminOverviewDto>>
{
    private readonly IApplicationDbContext _context;
    public GetScholarsAdminOverviewQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ScholarAdminOverviewDto>> Handle(GetScholarsAdminOverviewQuery request, CancellationToken cancellationToken)
    {
        var scholars = await _context.Scholars.AsNoTracking().ToListAsync(cancellationToken);
        var users = await _context.DomainUsers.AsNoTracking().ToListAsync(cancellationToken);
        var schools = await _context.Schools.AsNoTracking().ToListAsync(cancellationToken);
        var scholarships = await _context.Scholarships.AsNoTracking().ToListAsync(cancellationToken);

        var userById = users.ToDictionary(u => u.Id);
        var schoolById = schools.ToDictionary(s => s.Id);
        var scholarshipById = scholarships.ToDictionary(s => s.Id);

        return scholars
            .Select(s =>
            {
                userById.TryGetValue(s.UserId, out var user);
                schoolById.TryGetValue(s.SchoolId, out var school);
                scholarshipById.TryGetValue(s.ScholarshipId, out var scholarship);
                return new ScholarAdminOverviewDto(
                    s.Id,
                    UserDisplay.NameOf(user),
                    user?.Email ?? "—",
                    school?.Name ?? "—",
                    scholarship?.Name ?? "—",
                    s.BatchNumber,
                    s.DegreeProgram,
                    s.CurrentStatus.Status);
            })
            .OrderBy(s => s.FullName)
            .ToList();
    }
}
