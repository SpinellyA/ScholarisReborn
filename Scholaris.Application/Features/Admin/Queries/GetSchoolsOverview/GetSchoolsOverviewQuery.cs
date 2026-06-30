public record SchoolOverviewDto(
    Guid Id,
    string SchoolCode,
    string Name,
    Region Region,
    TermSystem TermSystem,
    int ActiveScholars,
    int? OpenTermNumber);

public record GetSchoolsOverviewQuery : IQuery<List<SchoolOverviewDto>>;
