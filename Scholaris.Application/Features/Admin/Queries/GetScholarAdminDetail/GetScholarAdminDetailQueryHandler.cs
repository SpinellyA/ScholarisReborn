using Microsoft.EntityFrameworkCore;

public class GetScholarAdminDetailQueryHandler : IQueryHandler<GetScholarAdminDetailQuery, ScholarAdminDetailDto?>
{
    private readonly IApplicationDbContext _context;
    public GetScholarAdminDetailQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<ScholarAdminDetailDto?> Handle(GetScholarAdminDetailQuery request, CancellationToken cancellationToken)
    {
        var scholar = await _context.Scholars.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ScholarId, cancellationToken);
        if (scholar is null)
            return null;

        var user = await _context.DomainUsers.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == scholar.UserId, cancellationToken);

        var schools = await _context.Schools.AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new OptionDto(s.Id, s.Name))
            .ToListAsync(cancellationToken);
        var scholarships = await _context.Scholarships.AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new OptionDto(s.Id, s.Name))
            .ToListAsync(cancellationToken);

        var history = scholar.Statuses
            .OrderByDescending(s => s.StartTime)
            .Select(s => new ScholarStatusHistoryDto(s.Status, s.StartTime, s.EndTime, s.Reason))
            .ToList();

        var profile = user?.Profile;

        return new ScholarAdminDetailDto(
            scholar.Id,
            scholar.UserId,
            user?.Email ?? string.Empty,
            scholar.SchoolId,
            scholar.ScholarshipId,
            scholar.BatchNumber,
            scholar.DegreeProgram,
            scholar.CurrentStatus.Status,
            profile?.FirstName ?? string.Empty,
            profile?.LastName ?? string.Empty,
            profile?.DateOfBirth,
            profile?.Address,
            profile?.ContactNumber,
            history,
            schools,
            scholarships);
    }
}
