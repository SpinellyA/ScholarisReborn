using Microsoft.EntityFrameworkCore;

public class GetInvitationByTokenQueryHandler : IQueryHandler<GetInvitationByTokenQuery, InvitationByTokenDto?>
{
    private readonly IApplicationDbContext _context;

    public GetInvitationByTokenQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<InvitationByTokenDto?> Handle(GetInvitationByTokenQuery request, CancellationToken cancellationToken)
    {
        var invitation = await _context.Invitations
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Token == request.Token, cancellationToken);

        if (invitation is null)
            return null;

        return new InvitationByTokenDto(
            invitation.Token,
            invitation.Email,
            invitation.Type,
            invitation.Status,
            invitation.SeedData?.FirstName,
            invitation.SeedData?.LastName,
            invitation.SeedData?.DateOfBirth,
            invitation.SeedData?.Address,
            invitation.SeedData?.ContactNumber);
    }
}
