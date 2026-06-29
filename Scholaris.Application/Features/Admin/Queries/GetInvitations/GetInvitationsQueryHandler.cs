using Microsoft.EntityFrameworkCore;

public class GetInvitationsQueryHandler : IQueryHandler<GetInvitationsQuery, List<InvitationListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInvitationsQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<List<InvitationListItemDto>> Handle(GetInvitationsQuery request, CancellationToken cancellationToken)
    {
        var invitations = await _context.Invitations.AsNoTracking().ToListAsync(cancellationToken);

        var adminIds = invitations.Select(i => i.InvitedByAdminId).Distinct().ToList();
        var schoolIds = invitations.Where(i => i.SchoolId.HasValue).Select(i => i.SchoolId!.Value).Distinct().ToList();
        var scholarshipIds = invitations.Where(i => i.ScholarshipId.HasValue).Select(i => i.ScholarshipId!.Value).Distinct().ToList();

        var admins = await _context.DomainUsers.AsNoTracking().Where(u => adminIds.Contains(u.Id)).ToListAsync(cancellationToken);
        var schools = await _context.Schools.AsNoTracking().Where(s => schoolIds.Contains(s.Id)).ToListAsync(cancellationToken);
        var scholarships = await _context.Scholarships.AsNoTracking().Where(s => scholarshipIds.Contains(s.Id)).ToListAsync(cancellationToken);

        return invitations
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InvitationListItemDto(
                i.Id,
                i.Email,
                i.Type,
                i.Status,
                i.InvitedByAdminId,
                admins.FirstOrDefault(a => a.Id == i.InvitedByAdminId)?.Email,
                i.SchoolId.HasValue ? schools.FirstOrDefault(s => s.Id == i.SchoolId)?.Name : null,
                i.ScholarshipId.HasValue ? scholarships.FirstOrDefault(s => s.Id == i.ScholarshipId)?.Name : null,
                i.CreatedAt,
                i.ExpiresAt))
            .ToList();
    }
}
