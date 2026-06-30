public record ScholarshipOverviewDto(
    Guid Id,
    string Name,
    string Description,
    int GrantCount,
    int ScholarCount);

public record GetScholarshipsOverviewQuery : IQuery<List<ScholarshipOverviewDto>>;
