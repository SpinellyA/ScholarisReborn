public record UserRegisteredEvent(Guid UserId, string Email) : DomainEvent;
public record InvitationCreatedEvent(Guid id, string Email, InvitationType type, Guid token) : DomainEvent;
public record InvitationAcceptedEvent(Guid id, string Email, InvitationType type) : DomainEvent;
public record InvitationRevokedEvent(Guid Id, string Email) : DomainEvent;


public record ScholarActivatedEvent(Guid ScholarId, Guid UserId) : DomainEvent;
public record ScholarTerminatedEvent(Guid ScholarId, string Reason) : DomainEvent;
public record ScholarWithheldEvent(Guid ScholarId, string Reason) : DomainEvent;
public record ScholarGraduatedEvent(Guid ScholarId) : DomainEvent;


public record TermRecordSubmittedEvent(Guid TermRecordId, Guid ScholarId) : DomainEvent;
public record TermRecordApprovedEvent(Guid TermRecordId, Guid ScholarId, Guid ProcessedByAdminId) : DomainEvent;
public record TermRecordDeferredEvent(Guid TermRecordId, Guid ScholarId, Guid ProcessedByAdminId, string Reason) : DomainEvent;
public record TermRecordDeniedEvent(Guid TermRecordId, Guid ScholarId, Guid ProcessedByAdminId, string Reason) : DomainEvent;


public record TermOpenedEvent(Guid TermId, Guid SchoolId, int TermNumber) : DomainEvent;
public record TermClosedEvent(Guid TermId, Guid SchoolId) : DomainEvent;

public record StipendDropAnnouncedEvent(Guid StipendDropId, Region Region, DateTime AnnouncedAt) : DomainEvent;
public record StipendReceiptConfirmedEvent(Guid StipendDropId, Guid ScholarId) : DomainEvent;
public record StipendReceiptDisputedEvent(Guid StipendDropId, Guid ScholarId) : DomainEvent;