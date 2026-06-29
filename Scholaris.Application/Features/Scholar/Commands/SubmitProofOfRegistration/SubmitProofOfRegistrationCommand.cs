using MediatR;

public record PorCourseInput(string CourseCode, double Units);

public record SubmitProofOfRegistrationCommand(
    Guid ScholarUserId,
    string FileName,
    string ContentType,
    byte[] FileContent,
    List<PorCourseInput> Courses) : ICommand;
