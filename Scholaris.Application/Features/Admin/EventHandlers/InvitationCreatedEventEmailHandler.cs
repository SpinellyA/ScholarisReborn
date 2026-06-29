using MediatR;
using Microsoft.Extensions.Configuration;

public class InvitationCreatedEventEmailHandler : INotificationHandler<DomainEventNotification<InvitationCreatedEvent>>
{
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public InvitationCreatedEventEmailHandler(IEmailSender emailSender, IConfiguration configuration)
    {
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task Handle(DomainEventNotification<InvitationCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var baseUrl = _configuration["AppBaseUrl"]?.TrimEnd('/') ?? string.Empty;
        var inviteLink = $"{baseUrl}/register?token={domainEvent.Token}";

        await _emailSender.SendInvitationEmailAsync(domainEvent.Email, inviteLink, domainEvent.Type, cancellationToken);
    }
}
