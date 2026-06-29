using MediatR;

public record GradeInput(string CourseCode, double Units, double Grade);

// The TCG document is optional (FileContent may be null/empty) but grades are required.
public record SubmitGradeTranscriptCommand(
    Guid ScholarUserId,
    string? FileName,
    string? ContentType,
    byte[]? FileContent,
    List<GradeInput> Grades) : ICommand;
