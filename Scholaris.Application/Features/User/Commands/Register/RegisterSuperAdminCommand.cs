using MediatR;

public record RegisterSuperAdminCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : ICommand;
