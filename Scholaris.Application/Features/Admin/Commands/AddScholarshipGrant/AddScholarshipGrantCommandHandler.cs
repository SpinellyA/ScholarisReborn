public class AddScholarshipGrantCommandHandler : ICommandHandler<AddScholarshipGrantCommand>
{
    private readonly IUnitOfWork _uow;

    public AddScholarshipGrantCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(AddScholarshipGrantCommand command, CancellationToken cancellationToken)
    {
        var scholarship = await _uow.ScholarshipRepository.GetById(command.ScholarshipId)
            ?? throw new DomainException("Scholarship not found.");

        scholarship.AddGrant(FinancialGrant.Create(command.GrantName, command.Amount));

        _uow.ScholarshipRepository.Update(scholarship);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
