public class DeleteSchoolCommandHandler : ICommandHandler<DeleteSchoolCommand>
{
    private readonly IUnitOfWork _uow;
    public DeleteSchoolCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteSchoolCommand command, CancellationToken cancellationToken)
    {
        var school = await _uow.SchoolRepository.GetById(command.Id)
            ?? throw new DomainException("School not found.");

        var scholars = await _uow.ScholarRepository.FindAsync(s => s.SchoolId == command.Id, cancellationToken);
        if (scholars.Count > 0)
            throw new DomainException("This school still has scholars and cannot be deleted.");

        _uow.SchoolRepository.Delete(school);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
