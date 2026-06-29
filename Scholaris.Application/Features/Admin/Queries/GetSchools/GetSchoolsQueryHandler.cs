using Microsoft.EntityFrameworkCore;

public class GetSchoolsQueryHandler : IQueryHandler<GetSchoolsQuery, List<SchoolListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSchoolsQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<List<SchoolListItemDto>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
        => await _context.Schools
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SchoolListItemDto(s.Id, s.SchoolCode, s.Name, s.Region, s.TermSystem))
            .ToListAsync(cancellationToken);
}
