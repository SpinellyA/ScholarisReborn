public class SetSchoolLogoCommandHandler : ICommandHandler<SetSchoolLogoCommand>
{
    private readonly IUnitOfWork _uow;
    public SetSchoolLogoCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(SetSchoolLogoCommand command, CancellationToken cancellationToken)
    {
        var school = await _uow.SchoolRepository.GetById(command.SchoolId)
            ?? throw new DomainException("School not found.");

        if (command.Content is { Length: > 0 })
            school.SetLogo(command.Content, command.ContentType);
        else
            school.RemoveLogo();

        await _uow.SaveChangesAsync(cancellationToken);
    }
}
