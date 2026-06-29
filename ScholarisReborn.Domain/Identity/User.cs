public enum UserStatusType
{
    Active,
    Suspended
}

public class User : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

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
            Email = email,
            PasswordHash = passwordHash,
        };

        user.RaiseEvent(new UserRegisteredEvent(user.Id, user.Email));
        return user;
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



//public class User : AggregateRoot
//{
//    public Guid Id { get; private set; }
//    public string Email { get; private set; } = string.Empty;
//    public string PasswordHash { get; private set; } = string.Empty;
//    public DateTime CreatedAt { get; private set; }

//    private User() { }

//    public static User Create(string email, string passwordHash)
//    {
//        if (string.IsNullOrWhiteSpace(email))
//            throw new DomainException("Email cannot be empty.");
//        if (string.IsNullOrWhiteSpace(passwordHash))
//            throw new DomainException("Password hash cannot be empty.");

//        var user = new User
//        {
//            Email = email,
//            PasswordHash = passwordHash,
//            CreatedAt = DateTime.UtcNow
//        };

//        user.RaiseEvent(new UserRegisteredEvent(user.Id, user.Email));
//        return user;
//    }
//}
