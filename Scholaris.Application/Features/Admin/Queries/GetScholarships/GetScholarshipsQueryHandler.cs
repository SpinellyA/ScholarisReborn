using Microsoft.EntityFrameworkCore;

public class GetScholarshipsQueryHandler : IQueryHandler<GetScholarshipsQuery, List<ScholarshipListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetScholarshipsQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<List<ScholarshipListItemDto>> Handle(GetScholarshipsQuery request, CancellationToken cancellationToken)
        => await _context.Scholarships
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new ScholarshipListItemDto(s.Id, s.Name, s.Description))
            .ToListAsync(cancellationToken);
}
