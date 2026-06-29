using MediatR;

public record InviteScholarCommand(
    Guid invitedByAdminId,
    string Email,
    Guid schoolId,
    Guid scholarshipId,
    int BatchNumber,
    string DegreeProgram,
    string? FirstName = null,
    string? LastName = null,
    DateOnly? DateOfBirth = null,
    string? Address = null,
    string? ContactNumber = null) : ICommand;
