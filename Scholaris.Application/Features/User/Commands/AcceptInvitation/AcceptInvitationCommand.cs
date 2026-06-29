using MediatR;

public record AcceptInvitationCommand(
    Guid Token,
    string Password,
    string? FirstName = null,
    string? LastName = null,
    DateTime? DateOfBirth = null,
    string? Address = null,
    string? ContactNumber = null) : ICommand;
