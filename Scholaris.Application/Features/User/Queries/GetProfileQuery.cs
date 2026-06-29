public record ProfileDto(
    string Email,
    string FirstName,
    string LastName,
    DateOnly? DateOfBirth,
    string? Address,
    string? ContactNumber);

public record GetProfileQuery(Guid UserId) : IQuery<ProfileDto?>;
