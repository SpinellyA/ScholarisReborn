using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

public class RecordProcessingTests
{
    private static DbContextOptions<MyDbContext> NewOptions()
        => new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public async Task Deny_WithWithhold_DeniesRecordAndWithholdsScholar()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();
        var adminId = Guid.NewGuid();

        Guid recordId, scholarId;

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var school = School.Create("SC1", "School", "d", Region.NCR, TermSystem.Semestral);
            var term = school.OpenTerm(1);
            uow.SchoolRepository.Add(school);

            var scholar = Scholar.Create(Guid.NewGuid(), school.Id, Guid.NewGuid(), 2024, "BS CS");
            scholarId = scholar.Id;
            uow.ScholarRepository.Add(scholar);

            var record = TermRecord.Create(scholarId, term.Id, gradesRequired: false);
            record.SubmitPOR(ProofOfRegistration.Create(Guid.NewGuid(), [Course.Create("CS101", 3)]));
            recordId = record.Id;
            uow.TermRepository.Add(record);

            await uow.SaveChangesAsync();
        }

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var handler = new DenyTermRecordCommandHandler(uow);
            await handler.Handle(new DenyTermRecordCommand(recordId, adminId, "Failed subjects", WithholdScholar: true), CancellationToken.None);
        }

        using (var ctx = new MyDbContext(options))
        {
            var record = await ctx.TermRecords.FirstAsync(r => r.Id == recordId);
            Assert.Equal(RecordStatus.Denied, record.Status);
            Assert.Equal(adminId, record.ProcessedByAdminId);

            var scholar = await ctx.Scholars.FirstAsync(s => s.Id == scholarId);
            Assert.Equal(ScholasticStatus.Withheld, scholar.CurrentStatus.Status);
        }
    }

    [Fact]
    public async Task Approve_SetsApprovedStatusAndProcessor()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();
        var adminId = Guid.NewGuid();
        Guid recordId;

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var record = TermRecord.Create(Guid.NewGuid(), Guid.NewGuid(), gradesRequired: false);
            record.SubmitPOR(ProofOfRegistration.Create(Guid.NewGuid(), [Course.Create("CS101", 3)]));
            recordId = record.Id;
            uow.TermRepository.Add(record);
            await uow.SaveChangesAsync();
        }

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            await new ApproveTermRecordCommandHandler(uow).Handle(new ApproveTermRecordCommand(recordId, adminId), CancellationToken.None);
        }

        using (var ctx = new MyDbContext(options))
        {
            var record = await ctx.TermRecords.FirstAsync(r => r.Id == recordId);
            Assert.Equal(RecordStatus.Approved, record.Status);
            Assert.Equal(adminId, record.ProcessedByAdminId);
        }
    }
}
