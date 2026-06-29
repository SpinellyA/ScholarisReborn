public class StipendDrop : AggregateRoot
{
    public Guid Id { get; private set; }
    public Region Region { get; private set; }
    public DateTime AnnouncedAt { get; private set; }

    private List<StipendReceipt> _receipts = new();
    public IReadOnlyCollection<StipendReceipt> Receipts => _receipts.AsReadOnly();

    private StipendDrop() { }

    public static StipendDrop Announce(Region region)
    {
        var drop = new StipendDrop
        {
            Id = Guid.CreateVersion7(),
            Region = region,
            AnnouncedAt = DateTime.UtcNow
        };
        drop.RaiseEvent(new StipendDropAnnouncedEvent(drop.Id, region, drop.AnnouncedAt));
        return drop;
    }

    public void ConfirmReceipt(Guid scholarId)
    {
        EnsureNoExistingReceipt(scholarId);
        _receipts.Add(StipendReceipt.Create(scholarId, received: true));
        RaiseEvent(new StipendReceiptConfirmedEvent(Id, scholarId));
    }

    public void DisputeReceipt(Guid scholarId)
    {
        EnsureNoExistingReceipt(scholarId);
        _receipts.Add(StipendReceipt.Create(scholarId, received: false));
        RaiseEvent(new StipendReceiptDisputedEvent(Id, scholarId));
    }

    private void EnsureNoExistingReceipt(Guid scholarId)
    {
        if (_receipts.Any(r => r.ScholarId == scholarId))
            throw new DomainException("Scholar has already responded to this stipend drop.");
    }
}

