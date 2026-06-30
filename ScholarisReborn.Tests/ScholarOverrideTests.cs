using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

public class ScholarOverrideTests
{
    private static DbContextOptions<MyDbContext> NewOptions()
        => new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public void ForceStatus_BypassesTransitionGuards()
    {
        var scholar = Scholar.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2024, "BS CS");
        scholar.Terminate("done");

        // Normal Activate() would throw from a Terminated state; ForceStatus must not.
        scholar.ForceStatus(ScholasticStatus.Active, "fixed mistake");

        Assert.Equal(ScholasticStatus.Active, scholar.CurrentStatus.Status);
        Assert.Equal("fixed mistake", scholar.CurrentStatus.Reason);
    }

    [Fact]
    public async Task OverrideScholar_UpdatesRecordProfileAndStatus()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();
        var userId = Guid.NewGuid();
        var newSchoolId = Guid.NewGuid();
        var newScholarshipId = Guid.NewGuid();
        Guid scholarId;

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            uow.UserRepository.Add(User.CreateWithId(userId, "s@x.com", UserProfile.Create("Old", "Name")));
            var scholar = Scholar.Create(userId, Guid.NewGuid(), Guid.NewGuid(), 2024, "BS CS");
            scholarId = scholar.Id;
            uow.ScholarRepository.Add(scholar);
            await uow.SaveChangesAsync();
        }

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            await new OverrideScholarCommandHandler(uow).Handle(new OverrideScholarCommand(
                scholarId, newSchoolId, newScholarshipId, 2025, "BS Math",
                ScholasticStatus.Withheld, "data fix",
                "New", "Name", null, null, null), CancellationToken.None);
        }

        using (var ctx = new MyDbContext(options))
        {
            var scholar = await ctx.Scholars.FirstAsync(s => s.Id == scholarId);
            Assert.Equal(newSchoolId, scholar.SchoolId);
            Assert.Equal(newScholarshipId, scholar.ScholarshipId);
            Assert.Equal(2025, scholar.BatchNumber);
            Assert.Equal("BS Math", scholar.DegreeProgram);
            Assert.Equal(ScholasticStatus.Withheld, scholar.CurrentStatus.Status);

            var user = await ctx.DomainUsers.FirstAsync(u => u.Id == userId);
            Assert.Equal("New", user.Profile!.FirstName);
        }
    }
}
