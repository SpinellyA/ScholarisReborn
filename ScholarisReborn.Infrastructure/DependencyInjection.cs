using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    // Identity registration (AddIdentityCore/AddSignInManager) is wired up in the Web project's
    // Program.cs instead of here, because SignInManager<T> lives in the ASP.NET Core shared
    // framework, which this class library doesn't reference.
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CHEDDB");
        services.AddDbContext<MyDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<MyDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmailSender, LoggingEmailSender>();
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddHostedService<RoleSeedingHostedService>();

        return services;
    }
}
