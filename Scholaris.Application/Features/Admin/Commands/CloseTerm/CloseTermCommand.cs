using MediatR;

public record CloseTermCommand(Guid SchoolId, Guid TermId) : ICommand;
