public class UpdateScholarshipCommandHandler : ICommandHandler<UpdateScholarshipCommand>
{
    private readonly IUnitOfWork _uow;
    public UpdateScholarshipCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateScholarshipCommand command, CancellationToken cancellationToken)
    {
        var scholarship = await _uow.ScholarshipRepository.GetById(command.Id)
            ?? throw new DomainException("Scholarship not found.");

        scholarship.UpdateDetails(command.Name, command.Description);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public class DeleteScholarshipCommandHandler : ICommandHandler<DeleteScholarshipCommand>
{
    private readonly IUnitOfWork _uow;
    public DeleteScholarshipCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteScholarshipCommand command, CancellationToken cancellationToken)
    {
        var scholarship = await _uow.ScholarshipRepository.GetById(command.Id)
            ?? throw new DomainException("Scholarship not found.");

        var scholars = await _uow.ScholarRepository.FindAsync(s => s.ScholarshipId == command.Id, cancellationToken);
        if (scholars.Count > 0)
            throw new DomainException("This scholarship still has scholars and cannot be deleted.");

        _uow.ScholarshipRepository.Delete(scholarship);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public class RemoveScholarshipGrantCommandHandler : ICommandHandler<RemoveScholarshipGrantCommand>
{
    private readonly IUnitOfWork _uow;
    public RemoveScholarshipGrantCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(RemoveScholarshipGrantCommand command, CancellationToken cancellationToken)
    {
        var scholarship = await _uow.ScholarshipRepository.GetById(command.ScholarshipId)
            ?? throw new DomainException("Scholarship not found.");

        scholarship.RemoveGrant(command.GrantName);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
