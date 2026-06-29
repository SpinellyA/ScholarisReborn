using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

public class StipendFlowTests
{
    private static DbContextOptions<MyDbContext> NewOptions()
        => new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public async Task Announce_ThenConfirm_RecordsReceiptForScholar()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();
        var userId = Guid.NewGuid();
        Guid scholarId, dropId;

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var school = School.Create("SC1", "School", "d", Region.NCR, TermSystem.Semestral);
            uow.SchoolRepository.Add(school);
            var scholar = Scholar.Create(userId, school.Id, Guid.NewGuid(), 2024, "BS CS");
            scholarId = scholar.Id;
            uow.ScholarRepository.Add(scholar);
            await uow.SaveChangesAsync();
        }

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            await new AnnounceStipendDropCommandHandler(uow).Handle(
                new AnnounceStipendDropCommand(Region.NCR, 7500, "October release"), CancellationToken.None);
        }

        using (var ctx = new MyDbContext(options))
        {
            dropId = (await ctx.StipendDrops.FirstAsync()).Id;
            var uow = new UnitOfWork(ctx, publisher.Object);
            await new ConfirmStipendReceiptCommandHandler(uow).Handle(
                new ConfirmStipendReceiptCommand(dropId, userId), CancellationToken.None);
        }

        using (var ctx = new MyDbContext(options))
        {
            var drop = await ctx.StipendDrops.FirstAsync(d => d.Id == dropId);
            var receipt = Assert.Single(drop.Receipts);
            Assert.Equal(scholarId, receipt.ScholarId);
            Assert.True(receipt.Received);
        }
    }

    [Fact]
    public async Task Confirm_Twice_Throws()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();
        var userId = Guid.NewGuid();

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var school = School.Create("SC1", "School", "d", Region.NCR, TermSystem.Semestral);
            uow.SchoolRepository.Add(school);
            uow.ScholarRepository.Add(Scholar.Create(userId, school.Id, Guid.NewGuid(), 2024, "BS CS"));
            var drop = StipendDrop.Announce(Region.NCR, 7500, "x");
            uow.StipendDropRepository.Add(drop);
            await uow.SaveChangesAsync();
        }

        Guid dropId;
        using (var ctx = new MyDbContext(options))
        {
            dropId = (await ctx.StipendDrops.FirstAsync()).Id;
            var uow = new UnitOfWork(ctx, publisher.Object);
            await new ConfirmStipendReceiptCommandHandler(uow).Handle(new ConfirmStipendReceiptCommand(dropId, userId), CancellationToken.None);
        }

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            await Assert.ThrowsAsync<DomainException>(() =>
                new DisputeStipendReceiptCommandHandler(uow).Handle(new DisputeStipendReceiptCommand(dropId, userId), CancellationToken.None));
        }
    }
}
