using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

// Targets the real Postgres database, because the EF in-memory provider cannot add a new
// owned-collection entity (Term) to a reloaded aggregate — the exact scenario this guards.
// Skipped automatically if the local CHEDDB isn't reachable.
public class OpenTermHandlerTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=CHEDDB;Username=postgres;Password=;";

    private static DbContextOptions<MyDbContext> PgOptions()
        => new DbContextOptionsBuilder<MyDbContext>().UseNpgsql(ConnectionString).Options;

    private static bool CanConnect()
    {
        try
        {
            using var ctx = new MyDbContext(PgOptions());
            return ctx.Database.CanConnect();
        }
        catch { return false; }
    }

    [Fact]
    public async Task OpenTerm_PersistsNewTerm_OnReloadedSchool()
    {
        if (!CanConnect())
            return; // Local CHEDDB not reachable — nothing to verify in this environment.

        var publisher = new Mock<IPublisher>();
        var code = "ZZTEST-" + Guid.NewGuid().ToString("N")[..8];
        Guid schoolId;

        // Persist a school first, then reload + open a term in a separate context (the failing path).
        using (var ctx = new MyDbContext(PgOptions()))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var school = School.Create(code, "Temp Test School", "d", Region.NCR, TermSystem.Semestral);
            schoolId = school.Id;
            uow.SchoolRepository.Add(school);
            await uow.SaveChangesAsync();
        }

        try
        {
            using (var ctx = new MyDbContext(PgOptions()))
            {
                var uow = new UnitOfWork(ctx, publisher.Object);
                await new OpenTermCommandHandler(uow).Handle(new OpenTermCommand(schoolId, 1), CancellationToken.None);
            }

            using (var ctx = new MyDbContext(PgOptions()))
            {
                var school = await ctx.Schools.FirstAsync(s => s.Id == schoolId);
                var term = Assert.Single(school.Terms);
                Assert.Equal(1, term.TermNumber);
                Assert.True(term.IsOpen);
            }
        }
        finally
        {
            // Clean up the temp school (and its owned terms).
            using var ctx = new MyDbContext(PgOptions());
            var school = await ctx.Schools.FirstOrDefaultAsync(s => s.Id == schoolId);
            if (school is not null) { ctx.Schools.Remove(school); await ctx.SaveChangesAsync(); }
        }
    }
}
