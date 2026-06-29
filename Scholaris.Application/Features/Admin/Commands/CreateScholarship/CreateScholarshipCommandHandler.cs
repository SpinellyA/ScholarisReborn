public class CreateScholarshipCommandHandler : ICommandHandler<CreateScholarshipCommand>
{
    private readonly IUnitOfWork _uow;

    public CreateScholarshipCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(CreateScholarshipCommand command, CancellationToken cancellationToken)
    {
        var scholarship = Scholarship.Create(command.Name, command.Description);

        _uow.ScholarshipRepository.Add(scholarship);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
