using MediatR;

public record CreateSchoolCommand(string SchoolCode, string Name, string Description, Region Region, TermSystem TermSystem) : ICommand;
