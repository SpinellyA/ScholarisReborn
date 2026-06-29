

using MediatR;

public record InviteScholarCommand(Guid invitedByAdminId, string Email, Guid scholarshipId) : ICommand;

