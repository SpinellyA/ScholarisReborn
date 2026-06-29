using Microsoft.EntityFrameworkCore;

public class GetBatchNumbersQueryHandler : IQueryHandler<GetBatchNumbersQuery, List<int>>
{
    private readonly IApplicationDbContext _context;

    public GetBatchNumbersQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<List<int>> Handle(GetBatchNumbersQuery request, CancellationToken cancellationToken)
    {
        var batches = await _context.Scholars.AsNoTracking()
            .Where(s => s.SchoolId == request.SchoolId)
            .Select(s => s.BatchNumber)
            .Distinct()
            .ToListAsync(cancellationToken);

        return batches.OrderByDescending(b => b).ToList();
    }
}
