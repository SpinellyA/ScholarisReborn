public record StipendDropDto(
    Guid Id,
    Region Region,
    double Amount,
    string Description,
    DateTime AnnouncedAt,
    int Confirmed,
    int Disputed);

public record GetStipendDropsQuery : IQuery<List<StipendDropDto>>;
