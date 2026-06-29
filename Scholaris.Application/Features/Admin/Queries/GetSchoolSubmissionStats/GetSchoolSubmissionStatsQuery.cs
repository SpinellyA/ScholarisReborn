public record SchoolSubmissionStatsDto(
    Guid Id,
    string Name,
    Region Region,
    int ActiveScholars,
    int PendingSubmissions);

public record GetSchoolSubmissionStatsQuery : IQuery<List<SchoolSubmissionStatsDto>>;
