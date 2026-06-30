using Microsoft.EntityFrameworkCore;

public class GetScholarshipByIdQueryHandler : IQueryHandler<GetScholarshipByIdQuery, ScholarshipDetailsDto?>
{
    private readonly IApplicationDbContext _context;
    public GetScholarshipByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<ScholarshipDetailsDto?> Handle(GetScholarshipByIdQuery request, CancellationToken cancellationToken)
    {
        var scholarship = await _context.Scholarships.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (scholarship is null)
            return null;

        var scholarCount = await _context.Scholars.AsNoTracking().CountAsync(s => s.ScholarshipId == request.Id, cancellationToken);

        return new ScholarshipDetailsDto(
            scholarship.Id,
            scholarship.Name,
            scholarship.Description,
            scholarCount,
            scholarship.Grants.Select(g => new GrantDto(g.Name, g.Amount)).ToList());
    }
}
