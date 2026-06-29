using Microsoft.Extensions.Logging;

public class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
        => _logger = logger;

    public Task SendInvitationEmailAsync(string toEmail, string inviteLink, InvitationType type, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[Invitation email - logged only, not actually sent] To: {Email} | Type: {Type} | Link: {Link}",
            toEmail, type, inviteLink);

        return Task.CompletedTask;
    }
}
