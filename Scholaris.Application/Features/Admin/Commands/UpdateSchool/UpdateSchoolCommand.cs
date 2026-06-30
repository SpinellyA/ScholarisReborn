using MediatR;

public record UpdateSchoolCommand(
    Guid Id,
    string SchoolCode,
    string Name,
    string Description,
    Region Region,
    TermSystem TermSystem) : ICommand;
