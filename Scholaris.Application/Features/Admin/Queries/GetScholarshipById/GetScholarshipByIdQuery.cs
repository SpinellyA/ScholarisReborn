public record GrantDto(string Name, double Amount);

public record ScholarshipDetailsDto(
    Guid Id,
    string Name,
    string Description,
    int ScholarCount,
    List<GrantDto> Grants);

public record GetScholarshipByIdQuery(Guid Id) : IQuery<ScholarshipDetailsDto?>;
