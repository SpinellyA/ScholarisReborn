public record ScholarAdminOverviewDto(
    Guid ScholarId,
    string FullName,
    string Email,
    string SchoolName,
    string ScholarshipName,
    int BatchNumber,
    string DegreeProgram,
    ScholasticStatus Status);

public record GetScholarsAdminOverviewQuery : IQuery<List<ScholarAdminOverviewDto>>;
