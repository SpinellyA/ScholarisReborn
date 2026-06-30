using MediatR;

public record UpdateScholarshipCommand(Guid Id, string Name, string Description) : ICommand;

public record DeleteScholarshipCommand(Guid Id) : ICommand;

public record RemoveScholarshipGrantCommand(Guid ScholarshipId, string GrantName) : ICommand;
