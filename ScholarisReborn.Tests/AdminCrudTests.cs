using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

public class AdminCrudTests
{
    private static DbContextOptions<MyDbContext> NewOptions()
        => new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public async Task DeleteSchool_WithScholars_Throws()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();
        Guid schoolId;

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var school = School.Create("SC1", "School", "d", Region.NCR, TermSystem.Semestral);
            schoolId = school.Id;
            uow.SchoolRepository.Add(school);
            uow.ScholarRepository.Add(Scholar.Create(Guid.NewGuid(), schoolId, Guid.NewGuid(), 2024, "BS CS"));
            await uow.SaveChangesAsync();
        }

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            await Assert.ThrowsAsync<DomainException>(() =>
                new DeleteSchoolCommandHandler(uow).Handle(new DeleteSchoolCommand(schoolId), CancellationToken.None));
        }
    }

    [Fact]
    public async Task UpdateSchool_ChangesDetails()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();
        Guid schoolId;

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var school = School.Create("SC1", "Old", "d", Region.NCR, TermSystem.Semestral);
            schoolId = school.Id;
            uow.SchoolRepository.Add(school);
            await uow.SaveChangesAsync();
        }

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            await new UpdateSchoolCommandHandler(uow).Handle(
                new UpdateSchoolCommand(schoolId, "SC2", "New Name", "desc", Region.CAR, TermSystem.Trimestral), CancellationToken.None);
        }

        using (var ctx = new MyDbContext(options))
        {
            var school = await ctx.Schools.FirstAsync(s => s.Id == schoolId);
            Assert.Equal("SC2", school.SchoolCode);
            Assert.Equal("New Name", school.Name);
            Assert.Equal(Region.CAR, school.Region);
            Assert.Equal(TermSystem.Trimestral, school.TermSystem);
        }
    }

    [Fact]
    public async Task DeleteScholarship_WithScholars_Throws()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();
        Guid scholarshipId;

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var scholarship = Scholarship.Create("Merit", "d");
            scholarshipId = scholarship.Id;
            uow.ScholarshipRepository.Add(scholarship);
            uow.ScholarRepository.Add(Scholar.Create(Guid.NewGuid(), Guid.NewGuid(), scholarshipId, 2024, "BS CS"));
            await uow.SaveChangesAsync();
        }

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            await Assert.ThrowsAsync<DomainException>(() =>
                new DeleteScholarshipCommandHandler(uow).Handle(new DeleteScholarshipCommand(scholarshipId), CancellationToken.None));
        }
    }
}
