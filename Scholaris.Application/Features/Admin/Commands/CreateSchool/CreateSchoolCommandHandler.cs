public class CreateSchoolCommandHandler : ICommandHandler<CreateSchoolCommand>
{
    private readonly IUnitOfWork _uow;

    public CreateSchoolCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(CreateSchoolCommand command, CancellationToken cancellationToken)
    {
        var school = School.Create(command.SchoolCode, command.Name, command.Description, command.Region, command.TermSystem);

        _uow.SchoolRepository.Add(school);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
