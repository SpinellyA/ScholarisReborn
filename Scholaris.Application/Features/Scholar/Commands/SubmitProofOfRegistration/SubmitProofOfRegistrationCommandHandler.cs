public class SubmitProofOfRegistrationCommandHandler : ICommandHandler<SubmitProofOfRegistrationCommand>
{
    private readonly IUnitOfWork _uow;

    public SubmitProofOfRegistrationCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(SubmitProofOfRegistrationCommand command, CancellationToken cancellationToken)
    {
        var (_, _, _, record) = await SubmissionResolver.ResolveOrCreateRecordAsync(_uow, command.ScholarUserId, cancellationToken);

        var file = StoredFile.Create(command.FileName, command.ContentType, command.FileContent, command.ScholarUserId);
        _uow.StoredFileRepository.Add(file);

        var courses = command.Courses
            .Select(c => Course.Create(c.CourseCode, c.Units))
            .ToList();

        record.SubmitPOR(ProofOfRegistration.Create(file.Id, courses));

        await _uow.SaveChangesAsync(cancellationToken);
    }
}
