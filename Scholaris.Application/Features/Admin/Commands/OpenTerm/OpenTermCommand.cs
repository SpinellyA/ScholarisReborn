using MediatR;

public record OpenTermCommand(Guid SchoolId, int TermNumber) : ICommand;
