using MediatR;

public record ConfirmStipendReceiptCommand(Guid DropId, Guid ScholarUserId) : ICommand;

public record DisputeStipendReceiptCommand(Guid DropId, Guid ScholarUserId) : ICommand;
