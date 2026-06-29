public record StipendReceipt(Guid ScholarId, bool Received, DateTime RespondedAt)
{
    public static StipendReceipt Create(Guid scholarId, bool received)
        => new(scholarId, received, DateTime.UtcNow);
}

