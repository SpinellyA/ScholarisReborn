public class ConfirmStipendReceiptCommandHandler : ICommandHandler<ConfirmStipendReceiptCommand>
{
    private readonly IUnitOfWork _uow;
    public ConfirmStipendReceiptCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(ConfirmStipendReceiptCommand command, CancellationToken cancellationToken)
    {
        var scholar = (await _uow.ScholarRepository.FindAsync(s => s.UserId == command.ScholarUserId, cancellationToken)).FirstOrDefault()
            ?? throw new DomainException("No scholar profile is associated with your account.");
        var drop = await _uow.StipendDropRepository.GetById(command.DropId)
            ?? throw new DomainException("Stipend announcement not found.");

        drop.ConfirmReceipt(scholar.Id);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public class DisputeStipendReceiptCommandHandler : ICommandHandler<DisputeStipendReceiptCommand>
{
    private readonly IUnitOfWork _uow;
    public DisputeStipendReceiptCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DisputeStipendReceiptCommand command, CancellationToken cancellationToken)
    {
        var scholar = (await _uow.ScholarRepository.FindAsync(s => s.UserId == command.ScholarUserId, cancellationToken)).FirstOrDefault()
            ?? throw new DomainException("No scholar profile is associated with your account.");
        var drop = await _uow.StipendDropRepository.GetById(command.DropId)
            ?? throw new DomainException("Stipend announcement not found.");

        drop.DisputeReceipt(scholar.Id);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
