public class OpenTermCommandHandler : ICommandHandler<OpenTermCommand>
{
    private readonly IUnitOfWork _uow;

    public OpenTermCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(OpenTermCommand command, CancellationToken cancellationToken)
    {
        var school = await _uow.SchoolRepository.GetById(command.SchoolId)
            ?? throw new DomainException("School not found.");

        school.OpenTerm(command.TermNumber);

        _uow.SchoolRepository.Update(school);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
