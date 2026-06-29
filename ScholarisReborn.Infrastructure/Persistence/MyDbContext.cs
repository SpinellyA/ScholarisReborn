using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<User> DomainUsers => Set<User>();
    public DbSet<Scholar> Scholars => Set<Scholar>();
    public DbSet<School> Schools => Set<School>();
    public DbSet<Scholarship> Scholarships => Set<Scholarship>();
    public DbSet<TermRecord> TermRecords => Set<TermRecord>();
    public DbSet<StipendDrop> StipendDrops => Set<StipendDrop>();
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);
    }
}
