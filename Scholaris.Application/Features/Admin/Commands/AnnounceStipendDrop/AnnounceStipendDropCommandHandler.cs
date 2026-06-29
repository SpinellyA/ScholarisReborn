public class AnnounceStipendDropCommandHandler : ICommandHandler<AnnounceStipendDropCommand>
{
    private readonly IUnitOfWork _uow;

    public AnnounceStipendDropCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(AnnounceStipendDropCommand command, CancellationToken cancellationToken)
    {
        var drop = StipendDrop.Announce(command.Region, command.Amount, command.Description);
        _uow.StipendDropRepository.Add(drop);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
