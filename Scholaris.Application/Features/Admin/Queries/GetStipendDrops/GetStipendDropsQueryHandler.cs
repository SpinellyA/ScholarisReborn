using Microsoft.EntityFrameworkCore;

public class GetStipendDropsQueryHandler : IQueryHandler<GetStipendDropsQuery, List<StipendDropDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStipendDropsQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<List<StipendDropDto>> Handle(GetStipendDropsQuery request, CancellationToken cancellationToken)
    {
        var drops = await _context.StipendDrops.AsNoTracking().ToListAsync(cancellationToken);

        return drops
            .OrderByDescending(d => d.AnnouncedAt)
            .Select(d => new StipendDropDto(
                d.Id, d.Region, d.Amount, d.Description, d.AnnouncedAt,
                d.Receipts.Count(r => r.Received),
                d.Receipts.Count(r => !r.Received)))
            .ToList();
    }
}
