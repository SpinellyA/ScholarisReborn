public enum InvitationStatus
{
    Pending,
    Accepted,
    Expired,
    Revoked
}

public record InvitationSeedData
{
    public string? FirstName { get; private init; }
    public string? LastName { get; private init; }
    public DateOnly? DateOfBirth { get; private init; }
    public string? Address { get; private init; }
    public string? ContactNumber { get; private init; }

    private InvitationSeedData() { }

    public static InvitationSeedData Create(
        string? firstName = null,
        string? lastName = null,
        DateOnly? dateOfBirth = null,
        string? address = null,
        string? contactNumber = null)
    {
        return new InvitationSeedData
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Address = address,
            ContactNumber = contactNumber
        };
    }
}

public class Invitation : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public InvitationType Type { get; private set; }
    public Guid InvitedByAdminId { get; private set; }
    public Guid Token { get; private set; } // what goes in the email link
    public Guid? SchoolId { get; private set; }
    public Guid? ScholarshipId { get; private set; }
    public InvitationSeedData? SeedData { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public InvitationStatus Status =>
        IsRevoked ? InvitationStatus.Revoked
        : IsUsed ? InvitationStatus.Accepted
        : DateTime.UtcNow > ExpiresAt ? InvitationStatus.Expired
        : InvitationStatus.Pending;

    public static Invitation Create(
        string email,
        InvitationType type,
        Guid invitedByAdminId,
        Guid? schoolId = null,
        Guid? scholarshipId = null,
        InvitationSeedData? seedData = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");

        if (type == InvitationType.Admin && (schoolId is not null || scholarshipId is not null))
            throw new DomainException("Admin invitations cannot be associated with a school or scholarship.");
        if (type == InvitationType.Scholar && schoolId is null)
            throw new DomainException("Scholar invitations must specify a school.");
        if (type == InvitationType.Scholar && scholarshipId is null)
            throw new DomainException("Scholar invitations must specify a scholarship.");

        var invitation = new Invitation
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            Type = type,
            InvitedByAdminId = invitedByAdminId,
            Token = Guid.CreateVersion7(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            SchoolId = schoolId,
            ScholarshipId = scholarshipId,
            SeedData = seedData,
        };

        invitation.RaiseEvent(new InvitationCreatedEvent(
            invitation.Id,
            invitation.Email,
            invitation.Type,
            invitation.Token));

        return invitation;
    }

    public void MarkAsUsed()
    {
        if (IsRevoked)
            throw new DomainException("Invitation has been revoked.");
        if (IsUsed)
            throw new DomainException("Invitation has already been used.");
        if (DateTime.UtcNow > ExpiresAt)
            throw new DomainException("Invitation has expired.");

        IsUsed = true;
        RaiseEvent(new InvitationAcceptedEvent(Id, Email, Type));
    }

    public void Revoke()
    {
        if (IsUsed)
            throw new DomainException("Cannot revoke an invitation that has already been used.");
        if (IsRevoked)
            throw new DomainException("Invitation has already been revoked.");

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RaiseEvent(new InvitationRevokedEvent(Id, Email));
    }
}
