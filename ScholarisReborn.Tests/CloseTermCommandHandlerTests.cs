using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

public class CloseTermCommandHandlerTests
{
    private DbContextOptions<MyDbContext> CreateNewContextOptions()
        => new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public async Task Handle_WithholdsActiveScholarsWithoutCompleteSubmission()
    {
        var options = CreateNewContextOptions();
        var mockPublisher = new Mock<IPublisher>();

        Guid schoolId;
        Guid termId;
        Guid submittedScholarId;
        Guid unsubmittedScholarId;

        using (var context = new MyDbContext(options))
        {
            var uow = new UnitOfWork(context, mockPublisher.Object);

            var school = School.Create("SC1", "Test School", "desc", Region.NCR, TermSystem.Semestral);
            var term = school.OpenTerm(2025, 1);
            schoolId = school.Id;
            termId = term.Id;
            uow.SchoolRepository.Add(school);

            var scholarshipId = Guid.NewGuid();

            var submittedScholar = Scholar.Create(Guid.NewGuid(), schoolId, scholarshipId, 2024, "BS Computer Science");
            submittedScholarId = submittedScholar.Id;
            uow.ScholarRepository.Add(submittedScholar);

            var unsubmittedScholar = Scholar.Create(Guid.NewGuid(), schoolId, scholarshipId, 2024, "BS Computer Science");
            unsubmittedScholarId = unsubmittedScholar.Id;
            uow.ScholarRepository.Add(unsubmittedScholar);

            var record = TermRecord.Create(submittedScholarId, termId, gradesRequired: true);
            record.SubmitTranscript(GradeTranscript.Create(Guid.NewGuid(), [CourseRecord.Create(Course.Create("CS101", 3), 1.0)]));
            record.SubmitPOR(ProofOfRegistration.Create(Guid.NewGuid(), [Course.Create("CS101", 3)]));
            uow.TermRepository.Add(record);

            await uow.SaveChangesAsync();
        }

        using (var context = new MyDbContext(options))
        {
            var uow = new UnitOfWork(context, mockPublisher.Object);
            var handler = new CloseTermCommandHandler(uow);

            await handler.Handle(new CloseTermCommand(schoolId, termId), CancellationToken.None);
        }

        using (var context = new MyDbContext(options))
        {
            var uow = new UnitOfWork(context, mockPublisher.Object);

            var submitted = (await uow.ScholarRepository.GetById(submittedScholarId))!;
            var unsubmitted = (await uow.ScholarRepository.GetById(unsubmittedScholarId))!;

            Assert.Equal(ScholasticStatus.Active, submitted.CurrentStatus.Status);
            Assert.Equal(ScholasticStatus.Withheld, unsubmitted.CurrentStatus.Status);
        }
    }
}
