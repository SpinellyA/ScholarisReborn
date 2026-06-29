using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IInvitationRepository InvitationRepository { get; }
    IUserRepository UserRepository { get; }
    ITermRepository TermRepository { get; }
    IScholarRepository ScholarRepository { get; }
    IScholarshipRepository ScholarshipRepository { get; }
    ISchoolRepository SchoolRepository { get; }
    IStipendDropRepository StipendDropRepository { get; }
    IStoredFileRepository StoredFileRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}