

using MediatR;

public record InviteAdminCommand(Guid invitedByAdminId, string Email) : ICommand;

