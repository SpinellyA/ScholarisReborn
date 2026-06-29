public enum UserStatusType
{
    Active,
    Suspended
}

public record UserProfile
{
    public string FirstName { get; private init; } = string.Empty;
    public string LastName { get; private init; } = string.Empty;
    public DateTime? DateOfBirth { get; private init; }
    public string? Address { get; private init; }
    public string? ContactNumber { get; private init; }

    private UserProfile() { }

    public static UserProfile Create(
        string firstName,
        string lastName,
        DateTime? dateOfBirth = null,
        string? address = null,
        string? contactNumber = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty.");

        return new UserProfile
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Address = address,
            ContactNumber = contactNumber
        };
    }
}

public class User : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;

    // Vestigial: credentials are owned by ASP.NET Core Identity (ApplicationUser) now.
    // Not written to by the registration flow; kept only so existing callers don't break.
    public string PasswordHash { get; private set; } = string.Empty;

    public UserProfile? Profile { get; private set; }

    private List<UserStatus> _statuses = new();
    public IReadOnlyCollection<UserStatus> Statuses => _statuses.AsReadOnly();

    private User() { }

    public static User Create(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash cannot be empty.");

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            PasswordHash = passwordHash,
        };

        user.RaiseEvent(new UserRegisteredEvent(user.Id, user.Email));
        return user;
    }

    // Used by registration, where Id must match the already-created ApplicationUser.Id.
    public static User CreateWithId(Guid id, string email, UserProfile? profile = null)
    {
        if (id == Guid.Empty)
            throw new DomainException("Id cannot be empty.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");

        var user = new User
        {
            Id = id,
            Email = email,
            Profile = profile,
        };

        user.RaiseEvent(new UserRegisteredEvent(user.Id, user.Email));
        return user;
    }

    public void SetProfile(UserProfile profile)
    {
        Profile = profile ?? throw new DomainException("Profile cannot be null.");
    }
}

public record UserStatus
{
    public UserStatusType Status { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    private UserStatus()
    {
    }
    public static UserStatus Create(UserStatusType status, DateTime? startTime = null)
    {
        return new UserStatus
        {
            Status = status,
            StartTime = startTime ?? DateTime.UtcNow,
        };
    }
    public UserStatus Close(DateTime? endTime) =>
        this with { EndTime = endTime ??  DateTime.UtcNow };
}
