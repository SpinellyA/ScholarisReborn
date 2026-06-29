public class RegisterSuperAdminCommandHandler : ICommandHandler<RegisterSuperAdminCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly IIdentityService _identityService;

    public RegisterSuperAdminCommandHandler(IUnitOfWork uow, IIdentityService identityService)
    {
        _uow = uow;
        _identityService = identityService;
    }

    public async Task Handle(RegisterSuperAdminCommand command, CancellationToken cancellationToken)
    {
        if (await _identityService.AnyUsersExistAsync(cancellationToken))
            throw new DomainException("Registration is invite-only once an administrator already exists.");

        var userId = Guid.CreateVersion7();

        await using var transaction = await _uow.BeginTransactionAsync(cancellationToken: cancellationToken);

        var createResult = await _identityService.CreateUserAsync(userId, command.Email, command.Password, cancellationToken);
        if (!createResult.Succeeded)
            throw new DomainException(string.Join(" ", createResult.Errors));

        var roleResult = await _identityService.AddToRoleAsync(userId, ApplicationRoles.SuperAdmin, cancellationToken);
        if (!roleResult.Succeeded)
            throw new DomainException(string.Join(" ", roleResult.Errors));

        var profile = UserProfile.Create(command.FirstName, command.LastName);
        var user = User.CreateWithId(userId, command.Email, profile);
        _uow.UserRepository.Add(user);

        await _uow.SaveChangesAsync(cancellationToken);
        await _uow.CommitAsync(cancellationToken);
    }
}
