using MediatR;

public record InviteScholarCommand(
    Guid invitedByAdminId,
    string Email,
    Guid schoolId,
    Guid scholarshipId,
    string? FirstName = null,
    string? LastName = null,
    DateTime? DateOfBirth = null,
    string? Address = null,
    string? ContactNumber = null) : ICommand;
