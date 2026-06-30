public record ScholarStatusHistoryDto(ScholasticStatus Status, DateTime StartTime, DateTime? EndTime, string? Reason);
public record OptionDto(Guid Id, string Name);

public record ScholarAdminDetailDto(
    Guid ScholarId,
    Guid UserId,
    string Email,
    // Scholar record
    Guid SchoolId,
    Guid ScholarshipId,
    int BatchNumber,
    string DegreeProgram,
    ScholasticStatus Status,
    // Profile
    string FirstName,
    string LastName,
    DateOnly? DateOfBirth,
    string? Address,
    string? ContactNumber,
    // History + option lists for the edit form
    List<ScholarStatusHistoryDto> StatusHistory,
    List<OptionDto> Schools,
    List<OptionDto> Scholarships);

public record GetScholarAdminDetailQuery(Guid ScholarId) : IQuery<ScholarAdminDetailDto?>;
