public record TermDto(Guid Id, int TermNumber, DateTime StartedAt, DateTime? EndedAt, bool IsOpen);

public record SchoolDetailsDto(
    Guid Id,
    string SchoolCode,
    string Name,
    string Description,
    Region Region,
    TermSystem TermSystem,
    List<TermDto> Terms,
    int ActiveScholarsCount);
