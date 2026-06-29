// A confidential uploaded document (PoR / TCG scan). Bytes live in the DB and are only ever
// served through an authenticated endpoint that checks the caller owns the file or is an admin.
public class StoredFile : AggregateRoot
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public byte[] Content { get; private set; } = Array.Empty<byte>();
    public Guid OwnerUserId { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private StoredFile() { }

    public static StoredFile Create(string fileName, string contentType, byte[] content, Guid ownerUserId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("File name cannot be empty.");
        if (content is null || content.Length == 0)
            throw new DomainException("File content cannot be empty.");
        if (ownerUserId == Guid.Empty)
            throw new DomainException("OwnerUserId cannot be empty.");

        return new StoredFile
        {
            Id = Guid.CreateVersion7(),
            FileName = fileName,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            Content = content,
            OwnerUserId = ownerUserId,
            UploadedAt = DateTime.UtcNow
        };
    }
}
