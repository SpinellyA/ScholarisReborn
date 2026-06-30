public record SchoolOverviewDto(
    Guid Id,
    string SchoolCode,
    string Name,
    Region Region,
    TermSystem TermSystem,
    bool HasLogo,
    int ActiveScholars,
    string? OpenTermLabel);

public record GetSchoolsOverviewQuery : IQuery<List<SchoolOverviewDto>>;
