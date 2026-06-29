public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand>
{
    private readonly IUnitOfWork _uow;

    public UpdateProfileCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await _uow.UserRepository.GetById(command.UserId)
            ?? throw new DomainException("User not found.");

        user.SetProfile(UserProfile.Create(
            command.FirstName,
            command.LastName,
            command.DateOfBirth,
            command.Address,
            command.ContactNumber));

        await _uow.SaveChangesAsync(cancellationToken);
    }
}
