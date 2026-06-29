public class SubmitGradeTranscriptCommandHandler : ICommandHandler<SubmitGradeTranscriptCommand>
{
    private readonly IUnitOfWork _uow;

    public SubmitGradeTranscriptCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(SubmitGradeTranscriptCommand command, CancellationToken cancellationToken)
    {
        var (_, _, _, record) = await SubmissionResolver.ResolveOrCreateRecordAsync(_uow, command.ScholarUserId, cancellationToken);

        Guid? fileId = null;
        if (command.FileContent is { Length: > 0 } && !string.IsNullOrWhiteSpace(command.FileName))
        {
            var file = StoredFile.Create(command.FileName, command.ContentType ?? "application/octet-stream", command.FileContent, command.ScholarUserId);
            _uow.StoredFileRepository.Add(file);
            fileId = file.Id;
        }

        var courseRecords = command.Grades
            .Select(g => CourseRecord.Create(Course.Create(g.CourseCode, g.Units), g.Grade))
            .ToList();

        record.SubmitTranscript(GradeTranscript.Create(fileId, courseRecords));

        await _uow.SaveChangesAsync(cancellationToken);
    }
}
