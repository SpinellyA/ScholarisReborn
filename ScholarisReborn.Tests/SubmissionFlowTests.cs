using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

public class SubmissionFlowTests
{
    private static DbContextOptions<MyDbContext> NewOptions()
        => new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public async Task FirstYear_SubmitsPorOnly_RecordIsCreatedAndApprovable()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();

        var userId = Guid.NewGuid();
        Guid schoolId, termId, scholarId;

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var school = School.Create("SC1", "School", "d", Region.NCR, TermSystem.Semestral);
            var term = school.OpenTerm(1);
            schoolId = school.Id; termId = term.Id;
            uow.SchoolRepository.Add(school);

            var scholar = Scholar.Create(userId, schoolId, Guid.NewGuid(), 2024, "BS CS");
            scholarId = scholar.Id;
            uow.ScholarRepository.Add(scholar);
            await uow.SaveChangesAsync();
        }

        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var handler = new SubmitProofOfRegistrationCommandHandler(uow);
            await handler.Handle(new SubmitProofOfRegistrationCommand(
                userId, "por.pdf", "application/pdf", new byte[] { 1, 2, 3 },
                [new PorCourseInput("CS101", 3)]), CancellationToken.None);
        }

        using (var ctx = new MyDbContext(options))
        {
            var record = await ctx.TermRecords.FirstAsync(r => r.ScholarId == scholarId && r.TermId == termId);
            Assert.False(record.GradesRequired);
            Assert.NotNull(record.ProofOfRegistration);
            Assert.Equal(RecordStatus.Pending, record.Status);
            Assert.Single(ctx.StoredFiles);

            // PoR alone is a complete submission for a first-year.
            record.Approve(Guid.NewGuid());
            Assert.Equal(RecordStatus.Approved, record.Status);
        }
    }

    [Fact]
    public async Task ReturningScholar_GetsPriorCoursesAndGradesRequired()
    {
        var options = NewOptions();
        var publisher = new Mock<IPublisher>();

        var userId = Guid.NewGuid();
        Guid schoolId, scholarId;

        // Term 1 (closed) has the scholar's PoR with CS101; term 2 is the current open term.
        using (var ctx = new MyDbContext(options))
        {
            var uow = new UnitOfWork(ctx, publisher.Object);
            var school = School.Create("SC1", "School", "d", Region.NCR, TermSystem.Semestral);
            var term1 = school.OpenTerm(1);
            school.CloseTerm(term1.Id);
            school.OpenTerm(2);
            schoolId = school.Id;
            uow.SchoolRepository.Add(school);

            var scholar = Scholar.Create(userId, schoolId, Guid.NewGuid(), 2024, "BS CS");
            scholarId = scholar.Id;
            uow.ScholarRepository.Add(scholar);

            var rec1 = TermRecord.Create(scholarId, term1.Id, gradesRequired: false);
            rec1.SubmitPOR(ProofOfRegistration.Create(Guid.NewGuid(), [Course.Create("CS101", 3)]));
            uow.TermRepository.Add(rec1);

            await uow.SaveChangesAsync();
        }

        using (var ctx = new MyDbContext(options))
        {
            var handler = new GetSubmissionContextQueryHandler(ctx);
            var dto = await handler.Handle(new GetSubmissionContextQuery(userId), CancellationToken.None);

            Assert.True(dto.HasOpenTerm);
            Assert.Equal(2, dto.TermNumber);
            Assert.False(dto.PorSubmitted); // nothing submitted for term 2 yet
            Assert.Single(dto.PriorCourses);
            Assert.Equal("CS101", dto.PriorCourses[0].CourseCode);
        }
    }
}
