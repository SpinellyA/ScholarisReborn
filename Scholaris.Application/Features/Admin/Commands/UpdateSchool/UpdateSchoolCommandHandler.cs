public class UpdateSchoolCommandHandler : ICommandHandler<UpdateSchoolCommand>
{
    private readonly IUnitOfWork _uow;
    public UpdateSchoolCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateSchoolCommand command, CancellationToken cancellationToken)
    {
        var school = await _uow.SchoolRepository.GetById(command.Id)
            ?? throw new DomainException("School not found.");

        school.UpdateDetails(command.SchoolCode, command.Name, command.Description, command.Region, command.TermSystem);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
