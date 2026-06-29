

using MediatR;

public record InviteScholarCommand(Guid invitedByAdminId, string Email, Guid schoolId, Guid scholarshipId) : ICommand;

