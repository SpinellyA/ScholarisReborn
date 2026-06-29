using MediatR;

public record InviteAdminCommand(
    Guid invitedByAdminId,
    string Email,
    string? FirstName = null,
    string? LastName = null,
    DateOnly? DateOfBirth = null,
    string? Address = null,
    string? ContactNumber = null) : ICommand;
