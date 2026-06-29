public class ApproveTermRecordCommandHandler : ICommandHandler<ApproveTermRecordCommand>
{
    private readonly IUnitOfWork _uow;
    public ApproveTermRecordCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(ApproveTermRecordCommand command, CancellationToken cancellationToken)
    {
        var record = await _uow.TermRepository.GetById(command.RecordId)
            ?? throw new DomainException("Record not found.");

        record.Approve(command.AdminUserId);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public class DeferTermRecordCommandHandler : ICommandHandler<DeferTermRecordCommand>
{
    private readonly IUnitOfWork _uow;
    public DeferTermRecordCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeferTermRecordCommand command, CancellationToken cancellationToken)
    {
        var record = await _uow.TermRepository.GetById(command.RecordId)
            ?? throw new DomainException("Record not found.");

        record.Defer(command.AdminUserId, command.Reason);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public class DenyTermRecordCommandHandler : ICommandHandler<DenyTermRecordCommand>
{
    private readonly IUnitOfWork _uow;
    public DenyTermRecordCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DenyTermRecordCommand command, CancellationToken cancellationToken)
    {
        var record = await _uow.TermRepository.GetById(command.RecordId)
            ?? throw new DomainException("Record not found.");

        record.Deny(command.AdminUserId, command.Reason);

        if (command.WithholdScholar)
        {
            var scholars = await _uow.ScholarRepository.FindAsync(s => s.Id == record.ScholarId, cancellationToken);
            var scholar = scholars.FirstOrDefault();
            if (scholar is not null && scholar.CurrentStatus.Status == ScholasticStatus.Active)
                scholar.Withhold(command.Reason);
        }

        await _uow.SaveChangesAsync(cancellationToken);
    }
}
