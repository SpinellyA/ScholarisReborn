public record InvitationByTokenDto(
    Guid Token,
    string Email,
    InvitationType Type,
    InvitationStatus Status,
    string? FirstName,
    string? LastName,
    DateOnly? DateOfBirth,
    string? Address,
    string? ContactNumber);

public record GetInvitationByTokenQuery(Guid Token) : IQuery<InvitationByTokenDto?>;
