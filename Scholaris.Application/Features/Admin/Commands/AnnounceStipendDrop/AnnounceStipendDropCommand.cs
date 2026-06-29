using MediatR;

public record AnnounceStipendDropCommand(Region Region, double Amount, string Description) : ICommand;
