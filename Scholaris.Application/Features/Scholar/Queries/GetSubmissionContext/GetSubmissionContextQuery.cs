public record PriorCourseDto(string CourseCode, double Units);

public record SubmissionContextDto(
    bool HasOpenTerm,
    int TermNumber,
    string SchoolName,
    bool PorSubmitted,
    bool GradesSubmitted,
    RecordStatus? Status,
    // Courses from the most recent prior-term PoR, to pre-fill the grade form (read-only Course+Units).
    // Empty ⇒ first-year scholar ⇒ grades are not required.
    List<PriorCourseDto> PriorCourses);

public record GetSubmissionContextQuery(Guid ScholarUserId) : IQuery<SubmissionContextDto>;
