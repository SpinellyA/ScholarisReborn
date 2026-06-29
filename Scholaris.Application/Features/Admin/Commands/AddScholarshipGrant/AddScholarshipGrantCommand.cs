using MediatR;

public record AddScholarshipGrantCommand(Guid ScholarshipId, string GrantName, double Amount) : ICommand;
