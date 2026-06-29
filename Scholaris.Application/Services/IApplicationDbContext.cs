using Microsoft.EntityFrameworkCore;

public interface IApplicationDbContext
{
    DbSet<Invitation> Invitations { get; }
    DbSet<User> DomainUsers { get; }
    DbSet<Scholar> Scholars { get; }
    DbSet<School> Schools { get; }
    DbSet<Scholarship> Scholarships { get; }
    DbSet<TermRecord> TermRecords { get; }
    DbSet<StipendDrop> StipendDrops { get; }
}
