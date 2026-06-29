public class TermRecord : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ScholarId { get; private set; }
    public Guid TermId { get; private set; }
    public RecordStatus Status { get; private set; }
    public Guid? ProcessedByAdminId { get; private set; }

    public GradeTranscript? GradeTranscript { get; private set; }
    public ProofOfRegistration? ProofOfRegistration { get; private set; }

    private TermRecord() { }

    public static TermRecord Create(Guid scholarId, Guid termId)
    {
        if (scholarId == Guid.Empty) throw new DomainException("ScholarId cannot be empty.");
        if (termId == Guid.Empty) throw new DomainException("TermId cannot be empty.");

        return new TermRecord
        {
            Id = Guid.CreateVersion7(),
            ScholarId = scholarId,
            TermId = termId,
            Status = RecordStatus.Pending
        };
    }

    public void SubmitTranscript(GradeTranscript transcript)
    {
        if (Status != RecordStatus.Pending && Status != RecordStatus.Deferred)
            throw new DomainException("Cannot submit transcript for a record that is already processed.");
        GradeTranscript = transcript ?? throw new DomainException("Transcript cannot be null.");
        TryMarkAsSubmitted();
    }

    public void SubmitPOR(ProofOfRegistration por)
    {
        if (Status != RecordStatus.Pending && Status != RecordStatus.Deferred)
            throw new DomainException("Cannot submit POR for a record that is already processed.");
        ProofOfRegistration = por ?? throw new DomainException("Proof of registration cannot be null.");
        TryMarkAsSubmitted();
    }

    private void TryMarkAsSubmitted()
    {
        if (GradeTranscript is not null && ProofOfRegistration is not null)
            RaiseEvent(new TermRecordSubmittedEvent(Id, ScholarId));
    }

    public void Approve(Guid adminId)
    {
        EnsureComplete();
        EnsureProcessable();
        Status = RecordStatus.Approved;
        ProcessedByAdminId = adminId;
        RaiseEvent(new TermRecordApprovedEvent(Id, ScholarId, adminId));
    }

    public void Defer(Guid adminId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A reason must be provided when deferring a record.");
        EnsureProcessable();
        Status = RecordStatus.Deferred;
        ProcessedByAdminId = adminId;
        RaiseEvent(new TermRecordDeferredEvent(Id, ScholarId, adminId, reason));
    }

    public void Deny(Guid adminId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A reason must be provided when denying a record.");
        EnsureComplete();
        EnsureProcessable();
        Status = RecordStatus.Denied;
        ProcessedByAdminId = adminId;
        RaiseEvent(new TermRecordDeniedEvent(Id, ScholarId, adminId, reason));
    }

    private void EnsureComplete()
    {
        if (GradeTranscript is null || ProofOfRegistration is null)
            throw new DomainException("Record must have both a grade transcript and proof of registration before processing.");
    }

    private void EnsureProcessable()
    {
        if (Status == RecordStatus.Approved || Status == RecordStatus.Denied)
            throw new DomainException($"Cannot process a record that is already '{Status}'.");
    }
}
