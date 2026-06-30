public record TermDto(Guid Id, int TermNumber, string Label, DateTime StartedAt, DateTime? EndedAt, bool IsOpen);

public record SchoolDetailsDto(
    Guid Id,
    string SchoolCode,
    string Name,
    string Description,
    Region Region,
    TermSystem TermSystem,
    bool HasLogo,
    int PeriodsPerYear,
    int SuggestedAcademicYearStart,
    int SuggestedPeriodNumber,
    List<TermDto> Terms,
    int ActiveScholarsCount);
