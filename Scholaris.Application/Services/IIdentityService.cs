public record AppIdentityResult(bool Succeeded, IReadOnlyCollection<string> Errors)
{
    public static readonly AppIdentityResult Success = new(true, []);

    public static AppIdentityResult Failure(IEnumerable<string> errors) => new(false, errors.ToList());
}

public interface IIdentityService
{
    Task<bool> AnyUsersExistAsync(CancellationToken cancellationToken = default);
    Task<AppIdentityResult> CreateUserAsync(Guid id, string email, string password, CancellationToken cancellationToken = default);
    Task<AppIdentityResult> AddToRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);
}
