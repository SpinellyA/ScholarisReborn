
public class Invitation : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public InvitationType Type { get; private set; }
    public Guid InvitedByAdminId { get; private set; }
    public Guid Token { get; private set; } // what goes in the email link
    public Guid? ScholarshipId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }


    public static Invitation Create(string email, 
        InvitationType type, 
        Guid invitedByAdminId,
        Guid? scholarshipId = null
        )
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");

        if (type == InvitationType.Admin && scholarshipId is not null)
            throw new DomainException("");
        if (type == InvitationType.Scholar && scholarshipId is null)
            throw new DomainException("");

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
            ScholarshipId = scholarshipId,
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
        if (IsUsed)
            throw new DomainException("Invitation has already been used.");
        if (DateTime.UtcNow > ExpiresAt)
            throw new DomainException("Invitation has expired.");

        IsUsed = true;
        RaiseEvent(new InvitationAcceptedEvent(Id, Email, Type));
    }
}