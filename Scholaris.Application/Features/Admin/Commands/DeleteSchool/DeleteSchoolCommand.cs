using MediatR;

public record DeleteSchoolCommand(Guid Id) : ICommand;
