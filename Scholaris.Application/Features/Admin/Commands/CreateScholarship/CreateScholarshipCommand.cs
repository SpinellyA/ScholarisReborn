using MediatR;

public record CreateScholarshipCommand(string Name, string Description) : ICommand;
