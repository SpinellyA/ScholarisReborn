public record RecordListItemDto(
    Guid RecordId,
    string ScholarName,
    string DegreeProgram,
    int BatchNumber,
    int TermNumber,
    RecordStatus Status,
    double? Gwa,
    bool PorSubmitted,
    bool GradesSubmitted,
    string? ProcessedByName);

public record GetRecordsForProcessingQuery(Guid SchoolId, int BatchNumber) : IQuery<List<RecordListItemDto>>;
