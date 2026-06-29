public interface IEmailSender
{
    Task SendInvitationEmailAsync(string toEmail, string inviteLink, InvitationType type, CancellationToken cancellationToken = default);
}
