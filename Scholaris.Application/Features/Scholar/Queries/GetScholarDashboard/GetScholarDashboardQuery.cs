public enum ComplianceState
{
    NoOpenTerm,
    NeedsSubmission,
    Submitted
}

public record DashboardStipendDto(Guid DropId, double Amount, string Description, DateTime AnnouncedAt, bool? Received);

public record ScholarDashboardDto(
    string FullName,
    int? Age,
    string SchoolName,
    string DegreeProgram,
    int BatchNumber,
    string ScholarshipName,
    string Email,
    ScholasticStatus Status,
    ComplianceState Compliance,
    int? OpenTermNumber,
    DashboardStipendDto? LatestStipend);

public record GetScholarDashboardQuery(Guid ScholarUserId) : IQuery<ScholarDashboardDto?>;
