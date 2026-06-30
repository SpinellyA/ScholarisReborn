using Microsoft.EntityFrameworkCore;

public class GetSchoolsOverviewQueryHandler : IQueryHandler<GetSchoolsOverviewQuery, List<SchoolOverviewDto>>
{
    private readonly IApplicationDbContext _context;
    public GetSchoolsOverviewQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<SchoolOverviewDto>> Handle(GetSchoolsOverviewQuery request, CancellationToken cancellationToken)
    {
        var schools = await _context.Schools.AsNoTracking().ToListAsync(cancellationToken);
        var scholars = await _context.Scholars.AsNoTracking().ToListAsync(cancellationToken);

        return schools
            .OrderBy(s => s.Name)
            .Select(school =>
            {
                var active = scholars.Count(s => s.SchoolId == school.Id && s.CurrentStatus.Status == ScholasticStatus.Active);
                var openTerm = school.Terms.FirstOrDefault(t => t.IsOpen);
                var openLabel = openTerm is null ? null : TermSystemInfo.Label(school.TermSystem, openTerm.AcademicYearStart, openTerm.PeriodNumber);
                return new SchoolOverviewDto(school.Id, school.SchoolCode, school.Name, school.Region, school.TermSystem, school.Logo is { Length: > 0 }, active, openLabel);
            })
            .ToList();
    }
}
