public record DetailCourseDto(string CourseCode, double Units);
public record DetailGradeDto(string CourseCode, double Units, double Grade);

public record TermRecordDetailDto(
    Guid RecordId,
    string ScholarName,
    string DegreeProgram,
    int BatchNumber,
    string SchoolName,
    int TermNumber,
    RecordStatus Status,
    bool GradesRequired,
    string? ProcessedByName,
    Guid? ProofOfRegistrationFileId,
    List<DetailCourseDto> RegistrationCourses,
    Guid? GradeTranscriptFileId,
    double? Gwa,
    List<DetailGradeDto> Grades);

public record GetTermRecordDetailQuery(Guid RecordId) : IQuery<TermRecordDetailDto?>;
