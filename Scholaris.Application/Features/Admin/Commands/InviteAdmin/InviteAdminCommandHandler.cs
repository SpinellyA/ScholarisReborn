public class InviteAdminCommandHandler : ICommandHandler<InviteScholarCommand>
{
    private readonly IUnitOfWork _uow;

    public InviteAdminCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(InviteScholarCommand command, CancellationToken cancellationToken)
    {
        var invitation = Invitation.Create(command.Email, InvitationType.Admin, command.invitedByAdminId);
        
        _uow.InvitationRepository.Add(invitation);
        await _uow.SaveChangesAsync();
    }
}


