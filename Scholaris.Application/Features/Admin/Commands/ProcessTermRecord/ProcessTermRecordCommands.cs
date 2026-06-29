using MediatR;

public record ApproveTermRecordCommand(Guid RecordId, Guid AdminUserId) : ICommand;

public record DeferTermRecordCommand(Guid RecordId, Guid AdminUserId, string Reason) : ICommand;

// WithholdScholar: optionally move the scholar to Withheld when denying (e.g. failed grades).
public record DenyTermRecordCommand(Guid RecordId, Guid AdminUserId, string Reason, bool WithholdScholar) : ICommand;
