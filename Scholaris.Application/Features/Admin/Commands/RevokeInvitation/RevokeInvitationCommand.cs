using MediatR;

public record RevokeInvitationCommand(Guid InvitationId) : ICommand;
