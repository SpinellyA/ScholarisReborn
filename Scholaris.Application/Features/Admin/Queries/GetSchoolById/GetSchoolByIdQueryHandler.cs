using Microsoft.EntityFrameworkCore;

public class GetSchoolByIdQueryHandler : IQueryHandler<GetSchoolByIdQuery, SchoolDetailsDto?>
{
    private readonly IApplicationDbContext _context;

    public GetSchoolByIdQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<SchoolDetailsDto?> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var school = await _context.Schools
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SchoolId, cancellationToken);

        if (school is null)
            return null;

        var activeScholarsCount = await _context.Scholars
            .AsNoTracking()
            .Where(s => s.SchoolId == request.SchoolId)
            .CountAsync(cancellationToken);

        var terms = school.Terms
            .OrderByDescending(t => t.StartedAt)
            .Select(t => new TermDto(t.Id, t.TermNumber, t.StartedAt, t.EndedAt, t.IsOpen))
            .ToList();

        return new SchoolDetailsDto(
            school.Id,
            school.SchoolCode,
            school.Name,
            school.Description,
            school.Region,
            school.TermSystem,
            terms,
            activeScholarsCount);
    }
}
