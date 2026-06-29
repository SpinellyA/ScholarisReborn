using MediatR;

public record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    DateOnly? DateOfBirth,
    string? Address,
    string? ContactNumber) : ICommand;
