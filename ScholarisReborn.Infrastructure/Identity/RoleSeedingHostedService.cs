using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class RoleSeedingHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RoleSeedingHostedService(IServiceScopeFactory scopeFactory)
        => _scopeFactory = scopeFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var roleName in ApplicationRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new ApplicationRole(roleName));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
