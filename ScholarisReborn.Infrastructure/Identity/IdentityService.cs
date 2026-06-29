using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> AnyUsersExistAsync(CancellationToken cancellationToken = default)
        => await _userManager.Users.AnyAsync(cancellationToken);

    public async Task<AppIdentityResult> CreateUserAsync(Guid id, string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            Id = id,
            Email = email,
            UserName = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        return result.Succeeded
            ? AppIdentityResult.Success
            : AppIdentityResult.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<AppIdentityResult> AddToRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new InvalidOperationException($"User '{userId}' was not found.");

        var result = await _userManager.AddToRoleAsync(user, role);

        return result.Succeeded
            ? AppIdentityResult.Success
            : AppIdentityResult.Failure(result.Errors.Select(e => e.Description));
    }
}
