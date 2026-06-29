using Microsoft.EntityFrameworkCore;

public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, ProfileDto?>
{
    private readonly IApplicationDbContext _context;

    public GetProfileQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<ProfileDto?> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.DomainUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return null;

        var p = user.Profile;
        return new ProfileDto(
            user.Email,
            p?.FirstName ?? string.Empty,
            p?.LastName ?? string.Empty,
            p?.DateOfBirth,
            p?.Address,
            p?.ContactNumber);
    }
}
