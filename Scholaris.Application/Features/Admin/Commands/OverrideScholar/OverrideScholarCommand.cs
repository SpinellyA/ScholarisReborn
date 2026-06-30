using MediatR;

// Admin escape hatch: edit a scholar's record + profile and force their scholastic
// status, deliberately bypassing the normal status-transition guards. Used to correct
// system errors / data-entry mistakes — not part of the regular workflow.
public record OverrideScholarCommand(
    Guid ScholarId,
    Guid SchoolId,
    Guid ScholarshipId,
    int BatchNumber,
    string DegreeProgram,
    ScholasticStatus Status,
    string? StatusReason,
    string FirstName,
    string LastName,
    DateOnly? DateOfBirth,
    string? Address,
    string? ContactNumber) : ICommand;
