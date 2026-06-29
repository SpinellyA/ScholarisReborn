using MediatR;

public record AcceptInvitationCommand(
    Guid Token,
    string Password,
    string? FirstName = null,
    string? LastName = null,
    DateOnly? DateOfBirth = null,
    string? Address = null,
    string? ContactNumber = null) : ICommand;
